using FS.Core.NotificationDomain;

namespace FS.Persistence.Outbox.Shared.Interfaces;

public interface INotificationPipelineHandler
{
    bool CanHandle(Notification notification);
    
    Task HandleNotificationAsync(Notification notification, CancellationToken ct);
}