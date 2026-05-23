using FS.Application.EventLogic.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Core.AnimalAnnouncementBC.Events;
using FS.Core.Enums.Notifications;
using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Entities;
using FS.Core.NotificationDomain.Stores;

namespace FS.Application.MissingPetLogic.EventHandlers;

public class ReportSpottedDomainEventHandler(
    INotificationRepository notificationRepository,
    IMissingAnnouncementQueryService missingAnnouncementQueryService)
    : IDomainEventHandler<ReportSpottedDomainEvent>
{
    public async Task Handle(ReportSpottedDomainEvent domainEvent, CancellationToken ct)
    {
        var notification = Notification.Create(
            NotificationType.ReportSpotted,
            "Вашего питомца видели на улице",
            "Нажмите, чтобы посмотреть где",
            [NotificationChannel.Push],
            domainEvent.MissingAnnouncementId);
        
        var creatorDeviceIds = await missingAnnouncementQueryService.GetCreatorDevicesByAnnouncementIdAsync(
            domainEvent.MissingAnnouncementId, 
            ct);

        var deliveries = creatorDeviceIds.Select(id => NotificationDelivery.Create(
            notificationId: notification.Id,
            userDeviceId: id,
            channel: NotificationChannel.Push))
            .ToArray();
        notification.SetDeliveries(deliveries);

        //TODO: попробовать без saveChanges
        await notificationRepository.CreateAsync(notification, ct);
    }
}