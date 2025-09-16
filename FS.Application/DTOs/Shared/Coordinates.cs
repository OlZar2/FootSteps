using System.ComponentModel;
using NetTopologySuite.Geometries;

namespace FS.Application.DTOs.Shared;

public record Coordinates
{
    [Description("Долгота. Обязательно, issue REQUIRED.")]
    public required double Latitude { get; set; }
    [Description("Широта. Обязательно, issue REQUIRED.")]
    public required double Longitude { get; set; }

    public static Coordinates From(Point point)
    {
        return new Coordinates()
        {
            Longitude = point.X,
            Latitude = point.Y
        };
    }
}