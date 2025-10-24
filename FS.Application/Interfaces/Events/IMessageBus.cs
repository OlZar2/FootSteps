namespace FS.Application.Interfaces.Events;

public interface IMessageBus
{
    Task PublishEmbedRequestAsync(EmbedRequest req, CancellationToken ct = default);
}