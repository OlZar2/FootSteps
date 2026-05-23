using System.ComponentModel;
using FS.Core.Shared.ValueObjects;

namespace FS.Application.Shared.DTOs;

public record CoordinatesDto
{
    [Description("Долгота. Обязательно, issue REQUIRED.")]
    public double Latitude { get; set; }
    
    [Description("Широта. Обязательно, issue REQUIRED.")]
    public double Longitude { get; set; }
}