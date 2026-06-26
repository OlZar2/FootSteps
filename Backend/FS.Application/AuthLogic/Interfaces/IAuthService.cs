using FS.Application.AuthLogic.DTOs;

namespace FS.Application.AuthLogic.Interfaces;

public interface IAuthService
{
    Task RegisterUserAsync(RegisterData userRegisterData, CancellationToken ct);

    Task ResendEmailConfirmationAsync(string email, CancellationToken ct);

    Task ConfirmEmailAsync(Guid userId, string token, CancellationToken ct);
    
    Task<JwtData> LoginAsync(LoginData loginData, CancellationToken ct);

    Task<UserMainInfo> GetMeAsync(Guid userId, CancellationToken ct);
}
