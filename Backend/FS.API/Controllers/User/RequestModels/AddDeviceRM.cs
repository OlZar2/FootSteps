namespace FS.API.Controllers.User.RequestModels;

public record AddDeviceRM
{
    public string? DeviceToken { get; init; }
}