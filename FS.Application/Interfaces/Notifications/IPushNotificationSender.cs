using FS.Application.DTOs.Notification;

namespace FS.Application.Interfaces.Notifications;

public interface IPushNotificationSender
{
    Task SendAsync(PushNotificationDto pushNotification);
}