using System.Text.Json;
using FS.Application.Interfaces.Events;
using FS.Core.Entities;
using FS.Persistence.Outbox.Handlers.Interfaces;

namespace FS.Persistence.Outbox.Handlers.Implementations;

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