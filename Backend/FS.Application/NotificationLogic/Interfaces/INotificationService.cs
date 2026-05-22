using FS.Core.AnimalAnnouncementBC.Events;

namespace FS.Application.NotificationLogic.Interfaces;

public interface INotificationService
{
    Task NotifyAboutNewMissingAnnouncementAsync(
        MissingAnnouncementCreatedDomainEvent @event,
        CancellationToken ct);
}