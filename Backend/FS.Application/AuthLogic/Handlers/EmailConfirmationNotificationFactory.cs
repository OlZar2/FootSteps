using FS.Application.AuthLogic.Configurations;
using FS.Core.Enums.Notifications;
using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Entities;

namespace FS.Application.AuthLogic.Handlers;

internal static class EmailConfirmationNotificationFactory
{
    public static Notification Create(
        Guid userId,
        string email,
        string emailConfirmationToken,
        EmailConfirmationOptions options)
    {
        var confirmationUrl = BuildConfirmationUrl(userId, emailConfirmationToken, options);
        var notification = Notification.Create(
            NotificationType.EmailConfirmation,
            "Подтверждение почты",
            $"Для завершения регистрации подтвердите почту: {confirmationUrl}",
            [NotificationChannel.Email],
            userId);

        var delivery = NotificationDelivery.CreateEmail(notification.Id, email);
        notification.SetDeliveries([delivery]);

        return notification;
    }

    private static string BuildConfirmationUrl(
        Guid userId,
        string emailConfirmationToken,
        EmailConfirmationOptions options)
    {
        return options.ConfirmationUrlTemplate
            .Replace("{userId}", Uri.EscapeDataString(userId.ToString()))
            .Replace("{token}", Uri.EscapeDataString(emailConfirmationToken));
    }
}
