using System.Text.Json;
using FS.Application.Interfaces.Events;
using FS.Core.OutboxDomain.Entities;
using FS.Persistence.Outbox.Shared.Interfaces;

namespace FS.Persistence.Outbox.Embeddings.Handlers;

public class ImageSearchRequestOutboxHandler(IOutboxHandler inner, IMessageBus messageBus) : IOutboxHandler
{
    public async Task HandleAsync(OutboxEvent outboxEvent, CancellationToken ct)
    {
        if (outboxEvent.Type != "image.search.request")
        {
            await inner.HandleAsync(outboxEvent, ct);
            return;
        }
        
        var req = JsonSerializer.Deserialize<SearchRequestEvent>(outboxEvent.Payload)!;
        await messageBus.PublishSearchRequestAsync(req, ct);
    }
}