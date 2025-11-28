using FS.Core.Enums.Notifications;

namespace FS.Application.DTOs.Notification;

public record PushNotificationDto
{
    public required string DeviceToken { get; init; }
    public required string Title { get; init; }
    public required string Body { get; init; }
    public Guid? EntityId { get; init; }
    public NotificationType Type { get; init; }
}