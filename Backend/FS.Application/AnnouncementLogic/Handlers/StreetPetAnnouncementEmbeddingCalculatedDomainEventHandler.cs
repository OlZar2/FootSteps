using System.Text.Json;
using FS.Application.EventLogic.Interfaces;
using FS.Application.Interfaces.Events;
using FS.Core.AnimalAnnouncementBC.Events;
using FS.Core.OutboxDomain.Entities;
using FS.Core.OutboxDomain.Stores;

namespace FS.Application.AnnouncementLogic.Handlers;

public class StreetPetAnnouncementEmbeddingCalculatedDomainEventHandler(IOutboxRepository outboxRepository) 
    : IDomainEventHandler<StreetPetAnnouncementEmbeddingCalculatedDomainEvent>
{
    public async Task Handle(StreetPetAnnouncementEmbeddingCalculatedDomainEvent domainEvent, CancellationToken ct)
    {
        var outboxPayload = JsonSerializer.Serialize(new EmbedRequest{
            ImageId = domainEvent.ImageId,
        });
        var outboxEvent = OutboxEvent.Create(
            "image.find.similar.missing",
            outboxPayload);
        await outboxRepository.AddAsync(outboxEvent, ct);
    }
}