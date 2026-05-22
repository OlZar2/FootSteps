using FS.Application.AuthLogic.DTOs;

namespace FS.Application.Interfaces.QueryServices;

public interface IUserQueryService
{
    Task<UserMainInfo> GetUserMainInfoByIdAsync(Guid id, CancellationToken ct);
}