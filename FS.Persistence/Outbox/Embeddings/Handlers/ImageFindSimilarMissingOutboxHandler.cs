using System.Text.Json;
using FS.Application.Interfaces.Events;
using FS.Application.Services.StreetPetAnnouncementLogic.Interfaces;
using FS.Core.Entities;
using FS.Persistence.Outbox.Shared.Interfaces;

namespace FS.Persistence.Outbox.Embeddings.Handlers;

public class ImageFindSimilarMissingOutboxHandler(
    IOutboxHandler inner,
    IStreetPetAnnouncementService streetPetAnnouncementService) : IOutboxHandler
{
    public async Task HandleAsync(OutboxEvent outboxEvent, CancellationToken ct)
    {
        if (outboxEvent.Type != "image.find.similar.missing")
        {
            await inner.HandleAsync(outboxEvent, ct);
            return;
        }
        
        var req = JsonSerializer.Deserialize<EmbedRequest>(outboxEvent.Payload)!;
        await streetPetAnnouncementService.UpdateSimilarAnnouncementAsync(req.ImageId, ct);
    }
}