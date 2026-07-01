using FS.Core.Shared.Abstractions;

namespace FS.Core.SearchDomain.Events;

public sealed record SearchRequestCompletedDomainEvent(
    Guid SearchRequestId
) : IDomainEvent;