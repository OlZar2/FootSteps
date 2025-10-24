using FS.Core.Entities;

namespace FS.Persistence.Repositories;

public interface IOutboxRepository
{
    Task AddAsync(OutboxEvent outboxEvent, CancellationToken ct);
    
    Task PublishAsync(OutboxEvent outboxEvent, CancellationToken ct);
}