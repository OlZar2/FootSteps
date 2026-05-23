using System.ComponentModel;

namespace FS.API.Controllers.User.RequestModels;

public class UpdateUserLocationRM
{
    [Description("Долгота. Обязательно, issue REQUIRED.")]
    public required double Latitude { get; set; }
    [Description("Широта. Обязательно, issue REQUIRED.")]
    public required double Longitude { get; set; }
}