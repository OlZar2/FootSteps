using System.Text;
using System.Text.Json;
using FS.Application.Interfaces;
using FS.Application.Interfaces.Events;
using FS.Core.Entities;
using FS.Core.Stores;
using FS.Persistence.Repositories;
using FS.RabbitMq.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Pgvector;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FS.RabbitMq.Services;

public sealed class SearchResponseConsumer(
    ILogger<SearchResponseConsumer> logger,
    IServiceProvider serviceProvider,
    IOptions<ImageEmbeddingRabbitOptions> imageEmbeddingRabbitOptions,
    IOptions<RabbitMqOptions> rabbitMqOptions)
    : BackgroundService
{
    private IConnection? _conn;
    private IChannel? _ch;

    private readonly ImageEmbeddingRabbitOptions _img = imageEmbeddingRabbitOptions.Value;
    private readonly RabbitMqOptions _mq = rabbitMqOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_mq.Uri),
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
        };

        _conn = await factory.CreateConnectionAsync(ct).ConfigureAwait(false);
        _ch   = await _conn.CreateChannelAsync(cancellationToken: ct).ConfigureAwait(false);

        await _ch.ExchangeDeclareAsync(_img.ExchangeName, ExchangeType.Topic, durable: true, cancellationToken: ct) ;
        
        await _ch.QueueDeclareAsync(
            queue: _img.SearchResponseQueue,
            durable: true,
            exclusive: false,
            autoDelete: false, cancellationToken: ct) ;
        
        await _ch.QueueBindAsync(
            queue: _img.SearchResponseQueue,
            exchange: _img.ExchangeName,
            routingKey: _img.SearchResponseKey, cancellationToken: ct) ;

        await _ch.BasicQosAsync(0, prefetchCount: 1, global: false, cancellationToken: ct).ConfigureAwait(false);

        var consumer = new AsyncEventingBasicConsumer(_ch);
        consumer.ReceivedAsync += OnMessageAsync;

        await _ch.BasicConsumeAsync(
            queue: _img.SearchResponseQueue,
            autoAck: false,
            consumer: consumer, cancellationToken: ct);

        logger.LogInformation("SearchResponseConsumer started, queue={Queue}", _img.ResponseQueue);

        try
        {
            while (!ct.IsCancellationRequested)
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
        }
        catch (TaskCanceledException) { /* нормальное завершение */ }
    }
    
    private async Task OnMessageAsync(object sender, BasicDeliverEventArgs ea)
    {
        try
        {
            var json = Encoding.UTF8.GetString(ea.Body.Span);
            var res  = JsonSerializer.Deserialize<SearchResponse>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if (res is null)
            {
                logger.LogError("Failed to deserialize SearchResponse: {Json}", json);
                if (_ch is not null)
                    await _ch.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                return;
            }

            using var scope = serviceProvider.CreateScope();
            var transactionService = scope.ServiceProvider.GetRequiredService<ITransactionService>();

            await transactionService.ExecuteInTransactionAsync(async () =>
            {
                var searchRequestRepository = scope.ServiceProvider.GetRequiredService<ISearchRequestRepository>();
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();

                if (!Guid.TryParse(res.SearchId, out var searchId))
                {
                    logger.LogError("Invalid SearchId '{SearchId}' in message: {Json}", res.SearchId, json);
                    if (_ch is not null)
                        await _ch.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
                    return;
                }

                var entity = await searchRequestRepository.GetByIdAsync(searchId, CancellationToken.None);
                if (entity.Embedding == null)
                {
                    entity.Embedding = new Vector(res.Embedding);
                    await searchRequestRepository.UpdateAsync(entity, CancellationToken.None);
                }

                var jobPayload = JsonSerializer.Serialize(new SearchOutboxEvent{ SearchId = entity.Id });

                var outboxEvent = OutboxEvent.Create(
                    "image.search.match",
                    jobPayload
                );
                await outboxRepository.AddAsync(outboxEvent, CancellationToken.None);
            }, CancellationToken.None);
            
            if (_ch is not null)
                await _ch.BasicAckAsync(ea.DeliveryTag, multiple: false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to process message");
            if (_ch is not null)
                await _ch.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
        }
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        logger.LogInformation("Stopping SearchResponseConsumer...");
        try { if (_ch is not null) await _ch.CloseAsync(ct); } catch { }
        try { if (_conn is not null) await _conn.CloseAsync(ct); } catch { }
        await base.StopAsync(ct);
    }

    public override void Dispose()
    {
        try { _ch?.Dispose(); } catch { }
        try { _conn?.Dispose(); } catch { }
        base.Dispose();
    }
}