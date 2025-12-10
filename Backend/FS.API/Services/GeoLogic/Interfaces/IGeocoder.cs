using FS.Application.DTOs.Shared;

namespace FS.API.Services.GeoLogic.Interfaces;

public interface IGeocoder
{
    Task<string?> GetHouseOrNull(CoordinatesDto geoPoint, CancellationToken ct);
    
    Task<string?> GetStreetOrNull(CoordinatesDto geoPoint, CancellationToken ct);

    Task<string?> GetDistrictOrNull(CoordinatesDto geoPoint, CancellationToken ct);

    Task<string?> GetLocalityOrNull(CoordinatesDto geoPoint, CancellationToken ct);
}