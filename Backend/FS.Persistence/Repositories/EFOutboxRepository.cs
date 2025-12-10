using FS.Core.OutboxDomain.Entities;
using FS.Core.OutboxDomain.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class EFOutboxRepository(ApplicationDbContext context) : IOutboxRepository
{
    public async Task AddAsync(OutboxEvent outboxEvent, CancellationToken ct)
    {
        context.OutboxEvents.Add(outboxEvent);
        await context.SaveChangesAsync(ct);
    }
    
    public async Task AddRangeAsync(OutboxEvent[] outboxEvent, CancellationToken ct)
    {
        context.OutboxEvents.AddRange(outboxEvent);
        await context.SaveChangesAsync(ct);
    }

    public async Task PublishAsync(OutboxEvent outboxEvent, CancellationToken ct)
    {
        context.OutboxEvents.Update(outboxEvent);
        await context.SaveChangesAsync(ct);
    }

    public async Task<List<OutboxEvent>> GetEventsWithLock(int number, CancellationToken ct)
    {
        var events = await context.OutboxEvents
            .FromSqlInterpolated($"""
                                       SELECT * FROM "OutboxEvents"
                                       WHERE "PublishedUtc" IS NULL
                                       ORDER BY "Id"
                                       FOR UPDATE SKIP LOCKED
                                       LIMIT {number}
                                   """)
            .ToListAsync(ct);

        return events;
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await context.SaveChangesAsync(ct);
    }
}