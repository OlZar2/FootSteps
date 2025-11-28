using FS.Core.Entities;

namespace FS.Core.Stores;

public interface INotificationDeliveryRepository
{
    Task CreateRangeAsync(NotificationDelivery[] deliveries, CancellationToken ct);

    Task<NotificationDelivery[]> GetPushDeliveriesWiyhUserDevicesByNotificationIdAsync(
        Guid notificationId,
        CancellationToken ct);
    
    Task SaveChangesAsync(CancellationToken ct);
}