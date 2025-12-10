using FS.Application.Interfaces.Events;

namespace FS.Application.Interfaces.QueryServices;

public interface IAnimalAnnouncementQueryService
{
    Task<EmbedRequest[]> GetDataForEmbeddingRequestByAnnouncementId(Guid announcementId, CancellationToken ct);
}