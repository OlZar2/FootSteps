using FS.Application.AuthLogic.Configurations;
using FS.Application.EventLogic.Interfaces;
using FS.Core.NotificationDomain.Stores;
using FS.Core.UserDomain.Events;
using Microsoft.Extensions.Options;

namespace FS.Application.AuthLogic.Handlers;

public class EmailConfirmationRequestedDomainEventHandler(
    INotificationRepository notificationRepository,
    IOptions<EmailConfirmationOptions> options) : IDomainEventHandler<EmailConfirmationRequestedDomainEvent>
{
    public async Task Handle(EmailConfirmationRequestedDomainEvent domainEvent, CancellationToken ct)
    {
        var notification = EmailConfirmationNotificationFactory.Create(
            domainEvent.UserId,
            domainEvent.Email,
            domainEvent.EmailConfirmationToken,
            options.Value);

        await notificationRepository.CreateAsync(notification, ct);
    }
}
