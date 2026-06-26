using FS.Application.Interfaces.QueryServices;
using FS.Application.NotificationLogic.DTOs;
using FS.Core.Enums.Notifications;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFNotificationDeliveryQueryService(ApplicationDbContext context) : INotificationDeliveryQueryService
{
    public async Task<UserDeviceDto[]> GetDeviceTokensForPushNotificationsAsync(Guid notificationId, CancellationToken ct)
    {
        return await context.Notifications
            .AsNoTracking()
            .Where(n => n.Id == notificationId)
            .SelectMany(n => n.NotificationDeliveries)
            .Where(nd => nd.Status == DeliveryStatus.Pending || nd.Status == DeliveryStatus.Failed)
            .Join(context.UserDevices,
                delivery => delivery.UserDeviceId,
                device => device.Id,
                (delivery, device) => new UserDeviceDto
                {
                    DeliveryId = delivery.Id,
                    DeviceToken = device.DeviceToken,
                    IsActive = device.IsActive
                })
            .ToArrayAsync(ct);
    }
}