using FS.Core.Entities;

namespace FS.Persistence.Outbox.Handlers.Interfaces;

public interface IOutboxHandler
{
    Task HandleAsync(OutboxEvent outboxEvent, CancellationToken ct);
}