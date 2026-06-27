using FS.Application.EventLogic.Interfaces;
using FS.Core.NotificationDomain.Stores;
using FS.Core.UserDomain.Events;

namespace FS.Application.AuthLogic.Handlers;

public class EmailConfirmationRequestedDomainEventHandler(
    INotificationRepository notificationRepository) : IDomainEventHandler<EmailConfirmationRequestedDomainEvent>
{
    public async Task Handle(EmailConfirmationRequestedDomainEvent domainEvent, CancellationToken ct)
    {
        var notification = EmailConfirmationNotificationFactory.Create(
            domainEvent.UserId,
            domainEvent.Email,
            domainEvent.EmailConfirmationToken);

        await notificationRepository.CreateAsync(notification, ct);
    }
}
