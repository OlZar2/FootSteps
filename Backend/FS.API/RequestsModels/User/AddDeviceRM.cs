namespace FS.API.RequestsModels.User;

public record AddDeviceRM
{
    public string? DeviceToken { get; init; }
}