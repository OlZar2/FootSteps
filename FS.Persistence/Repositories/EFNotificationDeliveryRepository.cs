using FS.Core.Entities;
using FS.Core.Enums.Notifications;
using FS.Core.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class EFNotificationDeliveryRepository(ApplicationDbContext context) : INotificationDeliveryRepository
{
    public async Task CreateRangeAsync(NotificationDelivery[] deliveries, CancellationToken ct)
    {
        context.NotificationDeliveries.AddRange(deliveries);
        await context.SaveChangesAsync(ct);
    }

    public async Task<NotificationDelivery[]> GetPushDeliveriesWiyhUserDevicesByNotificationIdAsync(
        Guid notificationId,
        CancellationToken ct)
    {
        return await context.NotificationDeliveries
            .Include(nd => nd.User)
            .ThenInclude(u => u.UserDevices)
            .Where(nd => nd.NotificationId == notificationId && (nd.Status == DeliveryStatus.Pending || nd.Status == DeliveryStatus.Failed))
            .ToArrayAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await context.SaveChangesAsync(ct);
    }
}