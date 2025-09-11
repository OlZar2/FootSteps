using FS.Application.DTOs.AuthDTOs;

namespace FS.Application.Services.AuthLogic.Interfaces;

public interface IAuthService
{
    Task<CreatedUserDTO> RegisterUserAsync(RegisterDTO userRegisterDTO, CancellationToken ct);
}
