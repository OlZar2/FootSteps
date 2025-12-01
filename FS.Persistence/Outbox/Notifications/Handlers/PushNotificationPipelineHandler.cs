using FS.Application.DTOs.Notification;
using FS.Application.Exceptions;
using FS.Application.Interfaces.Notifications;
using FS.Core.Entities;
using FS.Core.Stores;
using FS.Persistence.Outbox.Shared.Interfaces;

namespace FS.Persistence.Outbox.Notifications.Handlers;

public class PushNotificationPipelineHandler(
    INotificationDeliveryRepository notificationDeliveryRepository,
    IPushNotificationSender pushNotificationSender) : INotificationPipelineHandler
{
    public async Task HandleNotificationAsync(Notification notification, CancellationToken ct)
    {
        var deliveries = await notificationDeliveryRepository
            .GetPushDeliveriesForHandlingWiyhUserDevicesByNotificationIdAsync(notification.Id, ct);
        var hasErrors = false;

        foreach (var delivery in deliveries)
        {
            var pushNotification = new PushNotificationDto
            {
                Title = notification.Subject,
                Body = notification.Text,
                DeviceToken = delivery.UserDevice.DeviceToken,
                EntityId = notification.TargetEntityId,
                Type = notification.Type,
            };

            try
            {
                await pushNotificationSender.SendAsync(pushNotification);
                delivery.MarkAsSent();
                await notificationDeliveryRepository.SaveChangesAsync(ct);
            }
            catch (NotificationDeliveryException ex)
            {
                delivery.MarkAsFailed();
                hasErrors = true;
            }
        }
        
        if (hasErrors)
        {
            throw new NotificationDeliveryException();
        }
    }
}