using FS.Application.Interfaces.Notifications;
using FS.Application.NotificationLogic.DTOs;
using FS.Application.NotificationLogic.Exceptions;
using FS.Core.Enums.Notifications;
using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Stores;
using FS.Persistence.Outbox.Shared.Interfaces;

namespace FS.Persistence.Outbox.Notifications.Handlers;

public class EmailNotificationPipelineHandler(
    INotificationRepository notificationRepository,
    IEmailNotificationSender emailNotificationSender,
    INotificationPipelineHandler next) : INotificationPipelineHandler
{
    public bool CanHandle(Notification notification)
    {
        return notification.Channels.Contains(NotificationChannel.Email);
    }

    public async Task HandleNotificationAsync(Notification notification, CancellationToken ct)
    {
        var hasErrors = false;

        if (CanHandle(notification))
        {
            var deliveries = notification.NotificationDeliveries
                .Where(d => d.Channel == NotificationChannel.Email &&
                            (d.Status == DeliveryStatus.Pending || d.Status == DeliveryStatus.Failed))
                .ToArray();

            foreach (var delivery in deliveries)
            {
                if (string.IsNullOrWhiteSpace(delivery.RecipientEmail))
                {
                    notification.MarkDeliveryAsFailed(delivery.Id);
                    hasErrors = true;
                    await notificationRepository.SaveChangesAsync(ct);
                    continue;
                }

                var email = new EmailNotificationDto
                {
                    RecipientEmail = delivery.RecipientEmail,
                    Subject = notification.Subject,
                    Body = notification.Text,
                };

                try
                {
                    await emailNotificationSender.SendAsync(email, ct);
                    notification.MarkDeliveryAsSent(delivery.Id);
                    await notificationRepository.SaveChangesAsync(ct);
                }
                catch (NotificationDeliveryException)
                {
                    notification.MarkDeliveryAsFailed(delivery.Id);
                    hasErrors = true;
                    await notificationRepository.SaveChangesAsync(ct);
                }
            }
        }

        await next.HandleNotificationAsync(notification, ct);

        if (hasErrors)
        {
            throw new NotificationDeliveryException();
        }
    }
}
