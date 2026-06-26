using FS.Core.Shared.Abstractions;

namespace FS.Core.UserDomain.Events;

public sealed record EmailConfirmationRequestedDomainEvent(
    Guid UserId,
    string Email,
    string EmailConfirmationToken) : IDomainEvent;
