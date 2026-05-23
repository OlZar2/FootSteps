using FS.Core.Shared.Abstractions;

namespace FS.Application.EventLogic.Interfaces;

public interface IDomainEventsDispatcher
{
    Task DispatchAsync(
        IEnumerable<IDomainEvent> domainEvents,
        CancellationToken cancellationToken = default);
}