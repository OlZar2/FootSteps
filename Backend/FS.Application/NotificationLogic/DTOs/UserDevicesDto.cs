namespace FS.Application.NotificationLogic.DTOs;

public record UserDeviceDto
{
    public required Guid DeliveryId { get; init; }
    public required string DeviceToken { get; init; }
    public bool IsActive { get; init; }
}