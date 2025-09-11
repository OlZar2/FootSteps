using System.ComponentModel;

namespace FS.API.RequestsModels.Auth;

public record LoginRM
{
    [Description("Email пользователя. Обязательно, issue REQUIRED. " +
                 " При неверном формате issue INVALID_FORMAT. Если такого пользователя нет issue: INVALID_CREDENTIALS.")]
    public string Email { get; init; }
    
    [Description("Пароль. Обязательный, issue REQUIRED. Если пароль неверный нет issue: INVALID_CREDENTIALS.")]
    public string Password { get; init; }
}