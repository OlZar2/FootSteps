using FS.Application.DTOs.AuthDTOs;
using FS.Application.DTOs.Shared;
using NetTopologySuite.Geometries;

namespace FS.Application.Interfaces.QueryServices;

public interface IUserQueryService
{
    Task<MeInfo> GetUserMainInfoByIdAsync(Guid id, CancellationToken ct);
}