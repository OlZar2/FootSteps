using FS.Application.NotificationLogic.DTOs;

namespace FS.Application.Interfaces.Notifications;

public interface IPushNotificationSender
{
    Task SendAsync(PushNotificationDto pushNotification);
}