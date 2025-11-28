using FS.Core.Entities;
using FS.Core.Stores;
using FS.Persistence.Context;

namespace FS.Persistence.Repositories;

public class EFNotificationRepository(ApplicationDbContext context) : INotificationRepository
{
    public async Task CreateAsync(Notification notification, CancellationToken ct)
    {
        context.Notifications.AddRange(notification);
        await context.SaveChangesAsync(ct);
    }
}