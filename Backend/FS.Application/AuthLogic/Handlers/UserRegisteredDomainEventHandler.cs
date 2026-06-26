using FS.Application.AuthLogic.Configurations;
using FS.Application.EventLogic.Interfaces;
using FS.Core.Enums.Notifications;
using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Entities;
using FS.Core.NotificationDomain.Stores;
using FS.Core.UserDomain.Events;
using Microsoft.Extensions.Options;

namespace FS.Application.AuthLogic.Handlers;

public class UserRegisteredDomainEventHandler(
    INotificationRepository notificationRepository,
    IOptions<EmailConfirmationOptions> options) : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken ct)
    {
        var confirmationUrl = BuildConfirmationUrl(domainEvent);
        var notification = Notification.Create(
            NotificationType.EmailConfirmation,
            "Подтверждение почты",
            $"Для завершения регистрации подтвердите почту: {confirmationUrl}",
            [NotificationChannel.Email],
            domainEvent.UserId);

        var delivery = NotificationDelivery.CreateEmail(notification.Id, domainEvent.Email);
        notification.SetDeliveries([delivery]);

        await notificationRepository.CreateAsync(notification, ct);
    }

    private string BuildConfirmationUrl(UserRegisteredDomainEvent domainEvent)
    {
        return options.Value.ConfirmationUrlTemplate
            .Replace("{userId}", Uri.EscapeDataString(domainEvent.UserId.ToString()))
            .Replace("{token}", Uri.EscapeDataString(domainEvent.EmailConfirmationToken));
    }
}
