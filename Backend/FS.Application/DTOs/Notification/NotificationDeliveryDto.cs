using FS.Core.Enums.Notifications;

namespace FS.Application.DTOs.Notification;

public class NotificationDeliveryDto
{
    public Guid NotificationId { get; init; }
    public Guid DeliveryId { get; init; }

    public string Type { get; init; } = null!;
    public string Subject { get; init; } = string.Empty;
    public string Text { get; init; } = null!;

    public NotificationRecipientDto? Recipient { get; init; } = null!;

    public NotificationChannel Channel { get; init; }
}