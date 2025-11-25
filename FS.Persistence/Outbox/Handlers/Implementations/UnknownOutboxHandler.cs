using FS.Core.Entities;
using FS.Persistence.Outbox.Handlers.Interfaces;
using Microsoft.Extensions.Logging;

namespace FS.Persistence.Outbox.Handlers.Implementations;

public class UnknownOutboxHandler(ILogger<UnknownOutboxHandler> log) : IOutboxHandler
{
    public Task HandleAsync(OutboxEvent e, CancellationToken ct)
    {
        log.LogError("Unknown outbox event type: {Type}", e.Type);
        return Task.CompletedTask;
    }
}