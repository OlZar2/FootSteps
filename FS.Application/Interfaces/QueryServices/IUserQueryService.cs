using FS.Application.DTOs.AuthDTOs;

namespace FS.Application.Interfaces.QueryServices;

public interface IUserQueryService
{
    Task<MeInfo> GetUserMainInfoByIdAsync(Guid id, CancellationToken ct);
}