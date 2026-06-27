using FS.Core.Enums.Notifications;
using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Entities;

namespace FS.Application.AuthLogic.Handlers;

internal static class EmailConfirmationNotificationFactory
{
    public static Notification Create(
        Guid userId,
        string email,
        string emailConfirmationToken)
    {
        var notification = Notification.Create(
            NotificationType.EmailConfirmation,
            "Подтверждение почты",
            emailConfirmationToken,
            [NotificationChannel.Email],
            userId);

        var delivery = NotificationDelivery.CreateEmail(notification.Id, email);
        notification.SetDeliveries([delivery]);

        return notification;
    }
}
