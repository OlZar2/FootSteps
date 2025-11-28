using FS.Persistence.Context;
using FS.Persistence.Outbox.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FS.Persistence.Outbox.Notifications;

public class NotificationsOutboxWorker(
    IServiceProvider sp,
    ILogger<NotificationsOutboxWorker> log)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var notifications = await db.Notifications
                    .FromSqlRaw("""
                                    SELECT * FROM "Notifications"
                                    WHERE "IsCompleted" IS FALSE
                                    ORDER BY "Id"
                                    FOR UPDATE SKIP LOCKED
                                    LIMIT 100
                                """)
                    .ToListAsync(ct);

                //TODO: надо сделать чтобы слишком часто не отправлялось если что-то failed
                if (notifications.Count == 0)
                {
                    await Task.Delay(5000, ct);
                    continue;
                }

                var notificationPipelineHandler = scope.ServiceProvider
                    .GetRequiredService<INotificationPipelineHandler>();
                
                foreach (var notification in notifications)
                {
                    try
                    {
                        await notificationPipelineHandler.HandleNotificationAsync(notification, ct);
                        notification.MarkAsCompleted();
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex, "Event publish failed for event {Id}", notification.Id);
                    }
                }
                await db.SaveChangesAsync(ct);
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                log.LogError(ex, "Outbox loop error");
                await Task.Delay(1000, ct);
            }
        }
    }
}