using FS.Core.NotificationDomain;
using FS.Core.NotificationDomain.Stores;
using FS.Persistence.Context;

namespace FS.Persistence.Repositories;

public class EFNotificationRepository(ApplicationDbContext context) : INotificationRepository
{
    public async Task CreateAsync(Notification notification, CancellationToken ct)
    {
        context.Notifications.Add(notification);
        await context.SaveChangesAsync(ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await context.SaveChangesAsync(ct);
    }
}