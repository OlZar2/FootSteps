using FS.Core.OutboxDomain.Entities;

namespace FS.Core.OutboxDomain.Stores;

public interface IOutboxRepository
{
    Task AddAsync(OutboxEvent outboxEvent, CancellationToken ct);

    Task AddRangeAsync(OutboxEvent[] outboxEvent, CancellationToken ct);
    
    Task PublishAsync(OutboxEvent outboxEvent, CancellationToken ct);

    Task<List<OutboxEvent>> GetEventsWithLock(int number, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}