using System.Text.Json;
using FS.Application.Interfaces.Events;
using FS.Application.Services.SearchLogic.Interfaces;
using FS.Core.Entities;
using FS.Persistence.Outbox.Handlers.Interfaces;

namespace FS.Persistence.Outbox.Handlers.Implementations;

public class ImageSearchMatchOutboxHandler(IOutboxHandler inner, ISearchService searchService) : IOutboxHandler
{
    public async Task HandleAsync(OutboxEvent outboxEvent, CancellationToken ct)
    {
        if (outboxEvent.Type != "image.search.match")
        {
            await inner.HandleAsync(outboxEvent, ct);
            return;
        }
        
        var req = JsonSerializer.Deserialize<SearchOutboxEvent>(outboxEvent.Payload)!;
        await searchService.DoSearch(req.SearchId, ct);
    }
}