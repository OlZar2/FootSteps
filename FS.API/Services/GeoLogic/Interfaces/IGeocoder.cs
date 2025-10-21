using FS.Application.DTOs.Shared;

namespace FS.API.Services.GeoLogic.Interfaces;

public interface IGeocoder
{
    Task<string?> GetHouseOrNull(Coordinates geoPoint, CancellationToken ct);
    
    Task<string?> GetStreetOrNull(Coordinates geoPoint, CancellationToken ct);

    Task<string?> GetDistrictOrNull(Coordinates geoPoint, CancellationToken ct);

    Task<string?> GetLocalityOrNull(Coordinates geoPoint, CancellationToken ct);
}