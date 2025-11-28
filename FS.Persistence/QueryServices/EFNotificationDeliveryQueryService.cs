using FS.Application.DTOs.Notification;
using FS.Application.Interfaces.QueryServices;
using FS.Core.Enums.Notifications;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFNotificationDeliveryQueryService(ApplicationDbContext context) : INotificationDeliveryQueryService
{
    public async Task<PushNotificationDto[]> GetPushNotificationsAsync(Guid notificationId, CancellationToken ct)
    {
        var query =
            from delivery in context.NotificationDeliveries
            join notification in context.Notifications
                on delivery.NotificationId equals notification.Id
            join device in context.UserDevices
                on delivery.UserId equals device.UserId
            where delivery.NotificationId == notificationId
                  && delivery.Channel == NotificationChannel.Push
                  && delivery.Status == DeliveryStatus.Pending
            select new PushNotificationDto
            {
                DeviceToken = device.DeviceToken,
                Title       = notification.Subject,
                Body        = notification.Text,
                EntityId    = notification.TargetEntityId,
                Type        = notification.Type
            };

        return await query.ToArrayAsync(ct);
    }
}