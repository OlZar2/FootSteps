using System.Text.Json;
using FS.Application.EventLogic.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Core.AnimalAnnouncementBC.Events;
using FS.Core.OutboxDomain.Entities;
using FS.Core.OutboxDomain.Stores;

namespace FS.Application.AnnouncementLogic.Handlers;

public class AnnouncementCreatedDomainEventHandler(
    IAnimalAnnouncementQueryService animalAnnouncementQueryService,
    IOutboxRepository outboxRepository) 
    : IDomainEventHandler<AnnouncementCreatedDomainEvent>
{
    public async Task Handle(AnnouncementCreatedDomainEvent domainEvent, CancellationToken ct)
    {
        var embeddingRequests = await animalAnnouncementQueryService.
            GetDataForEmbeddingRequestByAnnouncementId(domainEvent.AnnouncementId, ct);
        
        var payloads = embeddingRequests.Select(er => 
            OutboxEvent.Create("image.embed.request",JsonSerializer.Serialize(er)))
            .ToArray();
        await outboxRepository.AddRangeAsync(payloads, ct);
    }
}