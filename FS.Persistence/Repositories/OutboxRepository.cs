using FS.Core.Entities;
using FS.Persistence.Context;

namespace FS.Persistence.Repositories;

public class OutboxRepository(ApplicationDbContext context) : IOutboxRepository
{
    public async Task AddAsync(OutboxEvent outboxEvent, CancellationToken ct)
    {
        context.OutboxEvents.Add(outboxEvent);
        await context.SaveChangesAsync(ct);
    }

    public async Task PublishAsync(OutboxEvent outboxEvent, CancellationToken ct)
    {
        context.OutboxEvents.Update(outboxEvent);
        await context.SaveChangesAsync(ct);
    }
}