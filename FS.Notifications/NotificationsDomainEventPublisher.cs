using FS.Application.Services.NotificationLogic.Interfaces;
using FS.Core.Abstractions;
using FS.Core.Events;

namespace FS.Notifications;

public class NotificationsDomainEventPublisher(
    INotificationService notificationService) : IDomainEventPublisher
{
    public async Task PublishAsync(IEnumerable<IDomainEvent> events, CancellationToken ct)
    {
        foreach (var ev in events)
        {
            switch (ev)
            {
                case MissingAnnouncementCreatedDomainEvent e:
                {
                    await notificationService.NotifyAboutNewMissingAnnouncementAsync(e, ct);
                    break;
                }
            }
        }
    }
}