using FS.Core.Shared.Abstractions;

namespace FS.Core.SearchDomain.Events;

public sealed record SearchRequestCompletedWithErrorDomainEvent(
    Guid SearchRequestId
) : IDomainEvent;