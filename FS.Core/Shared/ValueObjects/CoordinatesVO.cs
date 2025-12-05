using FS.Core.Exceptions;
using FS.Core.Shared.Abstractions;

namespace FS.Core.Shared.ValueObjects;

public class CoordinatesVO : ValueObject
{
    public double Latitude  { get; }
    public double Longitude { get; }

    private CoordinatesVO(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public static CoordinatesVO Create(double latitude, double longitude)
    {
        if (latitude < -90)
            throw new TooSmallException("latitude is too small",nameof(Latitude));
        
        if (latitude > 90)
            throw new TooLargeException("latitude is too large",nameof(Latitude));
        
        if (longitude < -180)
            throw new TooSmallException("longitude is too small",nameof(Longitude));
        
        if (longitude > 180)
            throw new TooLargeException("longitude is too large",nameof(Longitude));
        
        return new CoordinatesVO(latitude, longitude);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Latitude;
        yield return Longitude;
    }
}