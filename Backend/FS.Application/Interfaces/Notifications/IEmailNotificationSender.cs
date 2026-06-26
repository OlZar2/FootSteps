using FS.Application.NotificationLogic.DTOs;

namespace FS.Application.Interfaces.Notifications;

public interface IEmailNotificationSender
{
    Task SendAsync(EmailNotificationDto notification, CancellationToken ct);
}
