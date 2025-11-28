using FS.Core.Entities;

namespace FS.Persistence.Outbox.Shared.Interfaces;

public interface IOutboxHandler
{
    Task HandleAsync(OutboxEvent outboxEvent, CancellationToken ct);
}