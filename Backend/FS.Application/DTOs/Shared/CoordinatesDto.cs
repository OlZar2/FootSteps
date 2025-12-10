using System.ComponentModel;
using FS.Core.Shared.ValueObjects;

namespace FS.Application.DTOs.Shared;

public record CoordinatesDto
{
    [Description("Долгота. Обязательно, issue REQUIRED.")]
    public required double Latitude { get; set; }
    [Description("Широта. Обязательно, issue REQUIRED.")]
    public required double Longitude { get; set; }

    public static CoordinatesDto From(CoordinatesVO coordinates)
    {
        return new CoordinatesDto
        {
            Longitude = coordinates.Longitude,
            Latitude = coordinates.Latitude,
        };
    }
}