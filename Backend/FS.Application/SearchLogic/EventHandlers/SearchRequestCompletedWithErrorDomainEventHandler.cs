using FS.Application.EventLogic.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Core.Enums.Notifications;
using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Entities;
using FS.Core.NotificationDomain.Stores;
using FS.Core.SearchDomain.Events;

namespace FS.Application.SearchLogic.EventHandlers;

public class SearchRequestCompletedWithErrorDomainEventHandler(
    ISearchQueryService searchQueryService,
    INotificationRepository notificationRepository)
    : IDomainEventHandler<SearchRequestCompletedWithErrorDomainEvent>
{
    public async Task Handle(SearchRequestCompletedWithErrorDomainEvent domainEvent, CancellationToken ct)
    {
        var notification = Notification.Create(
            NotificationType.ReportFound,
            "Поиск похожих питомцев завершился с ошибкой",
            "Нажмите, чтобы узнать подробнее",
            [NotificationChannel.Push],
            domainEvent.SearchRequestId);
        
        var creatorDeviceIds = await searchQueryService.GetSearchCreatorDeviceIdsBySearchRequestId(
            domainEvent.SearchRequestId, 
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