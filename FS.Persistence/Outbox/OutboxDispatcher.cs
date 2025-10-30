using System.Text.Json;
using FS.Application.Interfaces.Events;
using FS.Application.Services.SearchLogic.Implementations;
using FS.Application.Services.SearchLogic.Interfaces;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FS.Persistence.Outbox;

public sealed class OutboxDispatcher(IServiceProvider sp, ILogger<OutboxDispatcher> log, IMessageBus bus)
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

                foreach (var e in events)
                {
                    try
                    {
                        if (e.Type == "image.embed.request")
                        {
                            var req = JsonSerializer.Deserialize<EmbedRequest>(e.Payload)!;
                            await bus.PublishEmbedRequestAsync(req, ct);
                        }
                        else if (e.Type == "image.search.match")
                        {
                            var searchService = scope.ServiceProvider.GetRequiredService<ISearchService>();
                            var req = JsonSerializer.Deserialize<SearchOutboxEvent>(e.Payload)!;
                            await searchService.DoSearch(req.SearchId, ct);
                        }
                        else if (e.Type == "image.search.request")
                        {
                            var req = JsonSerializer.Deserialize<SearchRequestEvent>(e.Payload)!;
                            await bus.PublishSearchRequestAsync(req, ct);
                        }
                        else
                        {
                            log.LogError("Unknown outbox event type: {EType}", e.Type);
                        }
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