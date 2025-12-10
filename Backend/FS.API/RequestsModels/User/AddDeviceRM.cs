namespace FS.API.RequestsModels.User;

public record AddDeviceRM
{
    public required string DeviceToken { get; init; }
}