using FirebaseAdmin.Messaging;
using FS.Application.DTOs.Notification;
using FS.Application.Exceptions;
using FS.Application.Interfaces.Notifications;
using FS.Firebase.Constants;
using Microsoft.Extensions.Logging;

namespace FS.Firebase.Senders;

public class FirebasePushNotificationSender(
    ILogger<FirebasePushNotificationSender> logger) : IPushNotificationSender
{
    public async Task SendAsync(PushNotificationDto pushNotification)
    {
        var message = new Message
        {
            Token = pushNotification.DeviceToken,
            Notification = new Notification
            {
                Title = pushNotification.Title,
                Body = pushNotification.Body,
            },
            Data = new Dictionary<string, string>
            {
                { NotificationDataKeys.Type, pushNotification.Type.ToString() },
                { NotificationDataKeys.EntityId, pushNotification.EntityId.ToString() ?? string.Empty }
            }
        };

        try
        {
            await FirebaseMessaging.DefaultInstance.SendAsync(message);
        }
        catch (FirebaseMessagingException ex)
        {
            logger.LogError(ex, "Error sending firebase notification");
            throw new NotificationDeliveryException();
        }
    }
}