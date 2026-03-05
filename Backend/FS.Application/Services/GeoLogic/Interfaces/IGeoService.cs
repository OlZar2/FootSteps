using FS.Application.DTOs.GeoDTOs;

namespace FS.Application.Services.GeoLogic.Interfaces;

public interface IGeoService
{
    Task<DistrictInfo[]> GetDistrictInfoAsync(string cityName, CancellationToken ct);
}