using FS.Application.Events;
using FS.Application.Services.NotificationLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC.Events;

namespace FS.Application.Services.AnnouncementLogic.Handlers;

public class MissingAnnouncementCreatedDomainEventHandler(
    INotificationService notificationService) : IDomainEventHandler<MissingAnnouncementCreatedDomainEvent>
{
    public async Task Handle(MissingAnnouncementCreatedDomainEvent domainEvent, CancellationToken ct)
    {
        await notificationService.NotifyAboutNewMissingAnnouncementAsync(domainEvent, ct);
    }
}