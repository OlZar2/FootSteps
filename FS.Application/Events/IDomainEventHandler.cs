using FS.Core.Shared.Abstractions;

namespace FS.Application.Events;

public interface IDomainEventHandler<in TEvent> 
    where TEvent : IDomainEvent
{
    Task Handle(TEvent domainEvent, CancellationToken ct);
}