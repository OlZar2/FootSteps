using FS.Application.DTOs.GeoDTOs;
using FS.Application.Interfaces.Geo;
using FS.Application.Services.GeoLogic.Interfaces;

namespace FS.Application.Services.GeoLogic.Implementations;

public class OverpassGeoService(IGeoRequestService geoRequestService) : IGeoService
{
    public async Task<DistrictInfo[]> GetDistrictInfoAsync(string cityName, CancellationToken ct)
    {
        throw new NotImplementedException();
        // try
        // {
        //     geoRequestService.GetDistrictInfoAsync(cityName, ct);
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e);
        //     throw;
        // }
    }
}