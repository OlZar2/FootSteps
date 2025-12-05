namespace FS.Application.DTOs.Notification;

public record UserDeviceDto
{
    public required Guid DeliveryId { get; init; }
    public required string DeviceToken { get; init; }
}