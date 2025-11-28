using FS.Core.Entities;

namespace FS.Core.Stores;

public interface IOutboxRepository
{
    Task AddAsync(OutboxEvent outboxEvent, CancellationToken ct);
    
    Task PublishAsync(OutboxEvent outboxEvent, CancellationToken ct);
}