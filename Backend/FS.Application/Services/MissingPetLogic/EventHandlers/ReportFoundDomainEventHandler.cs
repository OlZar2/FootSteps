using FS.Application.Events;
using FS.Application.Interfaces.QueryServices;
using FS.Core.AnimalAnnouncementBC.Events;
using FS.Core.Enums.Notifications;
using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Entities;
using FS.Core.NotificationDomain.Stores;

namespace FS.Application.Services.MissingPetLogic.EventHandlers;

public class ReportFoundDomainEventHandler(
    INotificationRepository notificationRepository,
    IMissingAnnouncementQueryService missingAnnouncementQueryService) : IDomainEventHandler<ReportFoundDomainEvent>
{
    public async Task Handle(ReportFoundDomainEvent domainEvent, CancellationToken ct)
    {
        var notification = Notification.Create(
            NotificationType.ReportFound,
            "Пользователь сообщил о находке вашего питомца",
            "Нажмите, чтобы посмотреть контакты",
            [NotificationChannel.Push],
            domainEvent.FoundUserId);
        
        var creatorDeviceIds = await missingAnnouncementQueryService.GetCreatorDevicesByAnnouncementIdAsync(
            domainEvent.AnnouncementId, 
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