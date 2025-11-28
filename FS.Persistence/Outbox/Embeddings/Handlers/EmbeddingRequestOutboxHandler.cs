using System.Text.Json;
using FS.Application.Interfaces.Events;
using FS.Core.Entities;
using FS.Persistence.Outbox.Shared.Interfaces;

namespace FS.Persistence.Outbox.Embeddings.Handlers;

public class EmbeddingRequestOutboxHandler(
    IOutboxHandler inner,
    IMessageBus messageBus) : IOutboxHandler
{
    public async Task HandleAsync(OutboxEvent outboxEvent, CancellationToken ct)
    {
        if (outboxEvent.Type != "image.embed.request")
        {
            await inner.HandleAsync(outboxEvent, ct);
            return;
        }
        
        var req = JsonSerializer.Deserialize<EmbedRequest>(outboxEvent.Payload)!;
        await messageBus.PublishEmbedRequestAsync(req, ct);
    }
}