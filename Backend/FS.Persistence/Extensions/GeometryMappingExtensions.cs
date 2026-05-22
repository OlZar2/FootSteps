using FS.Application.Shared.DTOs;
using NetTopologySuite.Geometries;

namespace FS.Persistence.Extensions;

public static class GeometryMappingExtensions
{
    public static CoordinatesDto ToCoordinatesDto(this Point point)
    {
        return new CoordinatesDto
        {
            Latitude = point.Y,
            Longitude = point.X
        };
    }
}