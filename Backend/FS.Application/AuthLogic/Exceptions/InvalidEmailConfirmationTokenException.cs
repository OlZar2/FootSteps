namespace FS.Application.AuthLogic.Exceptions;

public class InvalidEmailConfirmationTokenException(Guid userId)
    : Exception($"Email confirmation token for user '{userId}' is invalid.")
{
    public Guid UserId { get; } = userId;
}
