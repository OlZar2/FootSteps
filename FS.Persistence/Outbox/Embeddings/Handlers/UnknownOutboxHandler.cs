using FS.Core.OutboxDomain.Entities;
using FS.Persistence.Outbox.Shared.Interfaces;
using Microsoft.Extensions.Logging;

namespace FS.Persistence.Outbox.Embeddings.Handlers;

public class UnknownOutboxHandler(ILogger<UnknownOutboxHandler> log) : IOutboxHandler
{
    public Task HandleAsync(OutboxEvent e, CancellationToken ct)
    {
        log.LogError("Unknown outbox event type: {Type}", e.Type);
        return Task.CompletedTask;
    }
}