namespace FS.Application.AuthLogic.Exceptions;

public class UserForEmailConfirmationNotFoundException(Guid userId)
    : Exception($"Пользователь для подтверждения почты с id '{userId}' не найден.")
{
    public Guid UserId { get; } = userId;
}
