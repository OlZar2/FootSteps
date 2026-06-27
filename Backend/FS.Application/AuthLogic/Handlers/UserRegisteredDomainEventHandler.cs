using FS.Application.EventLogic.Interfaces;
using FS.Core.NotificationDomain.Stores;
using FS.Core.UserDomain.Events;

namespace FS.Application.AuthLogic.Handlers;

public class UserRegisteredDomainEventHandler(
    INotificationRepository notificationRepository) : IDomainEventHandler<UserRegisteredDomainEvent>
{
    public async Task Handle(UserRegisteredDomainEvent domainEvent, CancellationToken ct)
    {
        var notification = EmailConfirmationNotificationFactory.Create(
            domainEvent.UserId,
            domainEvent.Email,
            domainEvent.EmailConfirmationToken);

        await notificationRepository.CreateAsync(notification, ct);
    }
}
