using FS.Core.Entities;

namespace FS.Persistence.Outbox.Shared.Interfaces;

public interface INotificationPipelineHandler
{
    Task HandleNotificationAsync(Notification notification, CancellationToken ct);
}