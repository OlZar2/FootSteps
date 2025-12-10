using System.Text.Json;
using FS.Application.Services.NotificationLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC.Events;
using FS.Core.OutboxDomain.Entities;
using FS.Persistence.Outbox.Shared.Interfaces;

namespace FS.Persistence.Outbox.SharedOutbox.Handlers;

public class FindRecipientsForMissingAnnouncementNotificationOutboxHandler(
    IOutboxHandler inner,
    INotificationService notificationService) : IOutboxHandler
{
    public async Task HandleAsync(OutboxEvent outboxEvent, CancellationToken ct)
    {
        if (outboxEvent.Type != "notification.missing.recipients")
        {
            await inner.HandleAsync(outboxEvent, ct);
            return;
        }
        
        var @event = JsonSerializer.Deserialize<MissingAnnouncementCreatedDomainEvent>(outboxEvent.Payload)!;
        await notificationService.NotifyAboutNewMissingAnnouncementAsync(@event, ct);
    }
}