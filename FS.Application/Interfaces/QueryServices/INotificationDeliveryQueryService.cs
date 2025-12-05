using FS.Application.DTOs.Notification;

namespace FS.Application.Interfaces.QueryServices;

public interface INotificationDeliveryQueryService
{
    Task<UserDeviceDto[]> GetDeviceTokensForPushNotificationsAsync(Guid notificationId, CancellationToken ct);
}