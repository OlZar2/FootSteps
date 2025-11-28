using FS.Core.Abstractions;

namespace FS.Notifications;

public interface IDomainEventPublisher
{
    Task PublishAsync(IEnumerable<IDomainEvent> events, CancellationToken ct);
}