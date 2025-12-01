using FS.Application.DTOs.Shared;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Services.NotificationLogic.Interfaces;
using FS.Core.Entities;
using FS.Core.Enums.Notifications;
using FS.Core.Events;
using FS.Core.Stores;
using FS.Core.ValueObjects;

namespace FS.Application.Services.NotificationLogic.Implementations;

public class NotificationService(
    IUserQueryService userQueryService,
    IUserDeviceRepository userDeviceRepository,
    INotificationRepository notificationRepository,
    INotificationDeliveryRepository notificationDeliveryRepository) : INotificationService
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
        
        await notificationRepository.CreateAsync(notification, ct);

        var startSearchPoint = CoordinatesVO.Create(@event.CoordinatesVo.Latitude, @event.CoordinatesVo.Longitude);
        var userDevices = await userDeviceRepository
            .GetUserDevicesForMissingAnnouncementCreateNotificationAsync(startSearchPoint, 2000, @event.CreatorId, ct);
        
        var deliveries = new NotificationDelivery[userDevices.Length];
        foreach (var (userDevice, i) in userDevices.Select((value, i) => ( value, i )))
        {
            var notificationDelivery = NotificationDelivery.Create(
                notification.Id,
                userDevice,
                NotificationChannel.Push);
            deliveries[i] = notificationDelivery;
        }
        
        await notificationDeliveryRepository.CreateRangeAsync(deliveries, ct);
    }
}