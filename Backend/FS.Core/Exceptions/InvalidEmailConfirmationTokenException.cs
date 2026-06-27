using FS.Contracts.Error;

namespace FS.Core.Exceptions;

public class InvalidEmailConfirmationTokenException(Guid userId, string field)
    : DomainException(IssueCodes.InvalidValue, $"Email confirmation token for user '{userId}' is invalid.", field) { }