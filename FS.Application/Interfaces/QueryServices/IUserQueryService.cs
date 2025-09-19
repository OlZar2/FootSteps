using FS.Application.DTOs.AuthDTOs;

namespace FS.Application.Interfaces.QueryServices;

public interface IUserQueryService
{
    // Task<ProfileInfo> GetProfileInfoByIdAsync(string firstName, string secondName);

    Task<MeInfo> GetUserMainInfoByIdAsync(Guid id, CancellationToken ct);
}