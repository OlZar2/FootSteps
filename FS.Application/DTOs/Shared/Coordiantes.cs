using System.ComponentModel;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Application.DTOs.Shared;

public record Coordiantes
{
    [Description("Долгота. Обязательно, issue REQUIRED.")]
    public double Latitude { get; set; }
    [Description("Широта. Обязательно, issue REQUIRED.")]
    public double Longitude { get; set; }

    public static Coordiantes From(Point point)
    {
        return new Coordiantes()
        {
            Longitude = point.X,
            Latitude = point.Y
        };
    }
}