using FS.Application.Interfaces.QueryServices;
using FS.Application.NotificationLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC.Events;
using FS.Core.Enums.Notifications;
using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Entities;
using FS.Core.NotificationDomain.Stores;
using FS.Core.Shared.ValueObjects;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Application.NotificationLogic.Implementations;

public class NotificationService(
    IUserDeviceQueryService userDeviceQueryService,
    INotificationRepository notificationRepository,
    IMissingAnnouncementQueryService missingAnnouncementQueryService) : INotificationService
{
    public async Task NotifyAboutNewMissingAnnouncementAsync(
        MissingAnnouncementCreatedDomainEvent @event,
        CancellationToken ct)
    {
        var notification = Notification.Create(
            type: NotificationType.MissingAnnouncementCreated,
            subject: "Рядом потерялся питомец",
            text: "Новая потеряшка рядом с вами",
            allowedChannels: new List<NotificationChannel>
            {
                NotificationChannel.Push
            },
            targetEntityId: @event.AnnouncementId);

        var missingAnnouncementForNotifyData =
            await missingAnnouncementQueryService.GetDataForNotifyAsync(@event.AnnouncementId, ct);

        var startSearchPoint = CoordinatesVO.Create(
            missingAnnouncementForNotifyData.Coordinates.Latitude,
            missingAnnouncementForNotifyData.Coordinates.Longitude);
        
        //TODO: подумать над DI
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var centerPoint = geometryFactory.CreatePoint(new Coordinate(
            startSearchPoint.Longitude,
            startSearchPoint.Latitude));
        
        var userDeviceIds = await userDeviceQueryService
            .GetUserDevicesForMissingAnnouncementCreateNotificationAsync(
                centerPoint,
                2000,
                missingAnnouncementForNotifyData.CreatorId,
                ct);
        
        var deliveries = new NotificationDelivery[userDeviceIds.Length];
        foreach (var (deviceId, i) in userDeviceIds.Select((value, i) => ( value, i )))
        {
            var notificationDelivery = NotificationDelivery.Create(
                notification.Id,
                deviceId,
                NotificationChannel.Push);
            deliveries[i] = notificationDelivery;
        }
        notification.SetDeliveries(deliveries);
        
        await notificationRepository.CreateAsync(notification, ct);
    }
}