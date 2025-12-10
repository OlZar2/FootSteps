using FS.Application.DTOs.Notification;
using FS.Application.Exceptions;
using FS.Application.Interfaces.Notifications;
using FS.Application.Interfaces.QueryServices;
using FS.Core.Enums.Notifications;
using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Stores;
using FS.Persistence.Outbox.Shared.Interfaces;

namespace FS.Persistence.Outbox.Notifications.Handlers;

public class PushNotificationPipelineHandler(
    INotificationDeliveryQueryService notificationDeliveryQueryService,
    INotificationRepository notificationRepository,
    IPushNotificationSender pushNotificationSender) : INotificationPipelineHandler
{
    public bool CanHandle(Notification notification)
    {
        return notification.Channels.Contains(NotificationChannel.Push);
    }

    public async Task HandleNotificationAsync(Notification notification, CancellationToken ct)
    {
        if (CanHandle(notification))
        {
            var devices = await notificationDeliveryQueryService
                .GetDeviceTokensForPushNotificationsAsync(notification.Id, ct);
            var hasErrors = false;

            foreach (var device in devices)
            {
                var pushNotification = new PushNotificationDto
                {
                    Title = notification.Subject,
                    Body = notification.Text,
                    DeviceToken = device.DeviceToken,
                    EntityId = notification.TargetEntityId,
                    Type = notification.Type,
                };

                try
                {
                    await pushNotificationSender.SendAsync(pushNotification);
                    notification.MarkDeliveryAsSent(device.DeliveryId);
                    await notificationRepository.SaveChangesAsync(ct);
                }
                catch (NotificationDeliveryException ex)
                {
                    notification.MarkDeliveryAsFailed(device.DeliveryId);
                    hasErrors = true;
                    await notificationRepository.SaveChangesAsync(ct);
                }
            }
            
            if (hasErrors)
            {
                throw new NotificationDeliveryException();
            }
        }
    }
}