using FS.Core.Shared.Abstractions;

namespace FS.Core.UserDomain.Events;

public sealed record UserRegisteredDomainEvent(Guid UserId, string Email, string EmailConfirmationToken) : IDomainEvent;
