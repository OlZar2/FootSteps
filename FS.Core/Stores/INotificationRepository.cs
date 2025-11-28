using FS.Core.Entities;

namespace FS.Core.Stores;

public interface INotificationRepository
{
    Task CreateAsync(Notification notification, CancellationToken ct);
}