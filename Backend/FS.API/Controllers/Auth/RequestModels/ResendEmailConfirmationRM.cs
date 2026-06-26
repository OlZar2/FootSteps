using System.ComponentModel;

namespace FS.API.Controllers.Auth.RequestModels;

public record ResendEmailConfirmationRM
{
    [Description("Email пользователя. Обязательно, issue REQUIRED. При неверном формате issue INVALID_FORMAT.")]
    public required string Email { get; init; }
}
