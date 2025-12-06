using System.Text.Json;
using FS.Application.Events;
using FS.Application.Services.NotificationLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC.Events;
using FS.Core.OutboxDomain.Entities;
using FS.Core.OutboxDomain.Stores;

namespace FS.Application.Services.AnnouncementLogic.Handlers;

public class MissingAnnouncementCreatedDomainEventHandler(
    IOutboxRepository outboxRepository) : IDomainEventHandler<MissingAnnouncementCreatedDomainEvent>
{
    public async Task Handle(MissingAnnouncementCreatedDomainEvent domainEvent, CancellationToken ct)
    {
        var payload = JsonSerializer.Serialize(domainEvent);
        var outboxEvent = OutboxEvent.Create("notification.missing.recipients", payload);
        await outboxRepository.AddAsync(outboxEvent, ct);
    }
}