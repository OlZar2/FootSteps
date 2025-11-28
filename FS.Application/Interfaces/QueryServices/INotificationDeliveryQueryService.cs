using FS.Application.DTOs.Notification;

namespace FS.Application.Interfaces.QueryServices;

public interface INotificationDeliveryQueryService
{
    Task<PushNotificationDto[]> GetPushNotificationsAsync(Guid notificationId, CancellationToken ct);
}