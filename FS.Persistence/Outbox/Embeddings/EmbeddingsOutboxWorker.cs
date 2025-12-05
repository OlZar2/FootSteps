using FS.Core.OutboxDomain.Stores;
using FS.Persistence.Context;
using FS.Persistence.Outbox.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FS.Persistence.Outbox.Embeddings;

public sealed class EmbeddingsOutboxWorker(
    IServiceProvider sp,
    ILogger<EmbeddingsOutboxWorker> log)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = sp.CreateScope();
                var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
                var events = await outboxRepository.GetEventsWithLock(100, ct);

                if (events.Count == 0)
                {
                    await Task.Delay(5000, ct);
                    continue;
                }

                var outboxHandlerChain = scope.ServiceProvider.GetRequiredService<IOutboxHandler>();
                
                foreach (var e in events)
                {
                    try
                    {
                        await outboxHandlerChain.HandleAsync(e, ct);
                        e.MarkAsPublished();
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex, "Outbox publish failed for event {Id}", e.Id);
                    }
                }
                await outboxRepository.SaveChangesAsync(ct);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                log.LogError(ex, "Outbox loop error");
                await Task.Delay(1000, ct);
            }
        }
    }
}