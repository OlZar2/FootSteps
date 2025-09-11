using FS.Application.DTOs.AuthDTOs;

namespace FS.Application.Services.AuthLogic.Interfaces;

public interface IAuthService
{
    Task<CreatedUserData> RegisterUserAsync(RegisterData userRegisterData, CancellationToken ct);

    Task<JwtData> LoginAsync(LoginData loginData, CancellationToken ct);
}
