using FS.Application.Interfaces.Notifications;
using FS.Application.Interfaces.QueryServices;
using FS.Application.NotificationLogic.DTOs;
using FS.Application.NotificationLogic.Exceptions;
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
                if (!device.IsActive)
                {
                    notification.MarkDeliveryAsUnactual(device.DeliveryId);
                    await notificationRepository.SaveChangesAsync(ct);
                    continue;
                }
                
                //TODO: а если миллион пушей
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
                    //TODO: подумать о правильности вызова saveChanges после каждого пуша
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