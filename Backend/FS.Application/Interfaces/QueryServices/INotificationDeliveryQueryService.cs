using FS.Application.NotificationLogic.DTOs;

namespace FS.Application.Interfaces.QueryServices;

public interface INotificationDeliveryQueryService
{
    Task<UserDeviceDto[]> GetDeviceTokensForPushNotificationsAsync(Guid notificationId, CancellationToken ct);
}