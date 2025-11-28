using System.ComponentModel;
using NetTopologySuite.Geometries;

namespace FS.Application.DTOs.Shared;

public record CoordinatesDto
{
    [Description("Долгота. Обязательно, issue REQUIRED.")]
    public required double Latitude { get; set; }
    [Description("Широта. Обязательно, issue REQUIRED.")]
    public required double Longitude { get; set; }

    public static CoordinatesDto From(Point point)
    {
        return new CoordinatesDto()
        {
            Longitude = point.X,
            Latitude = point.Y
        };
    }
}