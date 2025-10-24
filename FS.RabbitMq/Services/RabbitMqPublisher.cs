using System.Text;
using System.Text.Json;
using FS.Application.Interfaces.Events;
using FS.RabbitMq.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace FS.RabbitMq.Services;

public sealed class RabbitMqPublisher(
    IOptions<ImageEmbeddingRabbitOptions> imageEmbeddingRabbitOptions,
    IOptions<RabbitMqOptions> rabbitMqOptions)
    : IMessageBus, IAsyncDisposable
{
    private readonly ImageEmbeddingRabbitOptions _imageEmbeddingRabbitOptions = imageEmbeddingRabbitOptions.Value;
    private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;

    private readonly SemaphoreSlim _pubLock = new(1, 1);

    private IConnection _conn = null!;
    private IChannel _ch = null!;
    private bool _initialized;

    private async Task EnsureInitializedAsync()
    {
        if (_initialized) return;

        var factory = new ConnectionFactory
        {
            Uri = new Uri(_rabbitMqOptions.Uri),
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true
        };

        _conn = await factory.CreateConnectionAsync().ConfigureAwait(false);
        _ch   = await _conn.CreateChannelAsync().ConfigureAwait(false);
        
        await _ch.ExchangeDeclareAsync(
            exchange: _imageEmbeddingRabbitOptions.ExchangeName,
            type: ExchangeType.Topic,
            durable: true,
            autoDelete: false,
            arguments: null
        ).ConfigureAwait(false);

        _initialized = true;
    }
    
    public async Task PublishEmbedRequestAsync(EmbedRequest req, CancellationToken ct = default)
    {
        await EnsureInitializedAsync().ConfigureAwait(false);

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(req));
        
        var props = new BasicProperties
        {
            DeliveryMode = DeliveryModes.Persistent,
            CorrelationId = req.ImageId,
            ContentType = "application/json",
        };
        
        await _pubLock.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            await _ch.BasicPublishAsync(
                exchange: _imageEmbeddingRabbitOptions.ExchangeName,
                routingKey: _imageEmbeddingRabbitOptions.RequestKey,
                mandatory: false,
                basicProperties: props,
                body: body, cancellationToken: ct
            ).ConfigureAwait(false);
        }
        finally
        {
            _pubLock.Release();
        }
    }

    public async ValueTask DisposeAsync()
    {
        try { if (_ch is not null) await _ch.CloseAsync().ConfigureAwait(false); } catch { /* ignore */ }
        try { if (_conn is not null) await _conn.CloseAsync().ConfigureAwait(false); } catch { /* ignore */ }

        if (_ch is IAsyncDisposable adCh) await adCh.DisposeAsync();
        _conn?.Dispose();
        _pubLock.Dispose();
    }
}