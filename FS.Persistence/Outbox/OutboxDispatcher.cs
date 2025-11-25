using FS.Persistence.Context;
using FS.Persistence.Outbox.Handlers.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FS.Persistence.Outbox;

public sealed class OutboxDispatcher(
    IServiceProvider sp,
    ILogger<OutboxDispatcher> log)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var events = await db.OutboxEvents
                    .FromSqlRaw("""
                                    SELECT * FROM "OutboxEvents"
                                    WHERE "PublishedUtc" IS NULL
                                    ORDER BY "Id"
                                    FOR UPDATE SKIP LOCKED
                                    LIMIT 100
                                """)
                    .ToListAsync(ct);

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
                await db.SaveChangesAsync(ct);
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