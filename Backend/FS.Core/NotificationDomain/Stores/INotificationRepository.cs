namespace FS.Core.NotificationDomain.Stores;

public interface INotificationRepository
{
    Task CreateAsync(Notification notification, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}