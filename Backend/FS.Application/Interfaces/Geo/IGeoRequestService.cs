using FS.Application.DTOs.GeoDTOs;

namespace FS.Application.Interfaces.Geo;

public interface IGeoRequestService
{
    Task<DistrictInfo[]> GetDistrictInfoAsync(string cityName, CancellationToken ct);
}