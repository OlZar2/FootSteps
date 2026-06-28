using FS.Application.Interfaces.Events;
using FS.Core.AnimalAnnouncementBC.Enums;

namespace FS.Application.Interfaces.QueryServices;

public interface IAnimalAnnouncementQueryService
{
    Task<DeleteType?> GetDeleteTypeByIdAsync(Guid announcementId, CancellationToken ct);

    Task<EmbedRequest[]> GetDataForEmbeddingRequestByAnnouncementId(Guid announcementId, CancellationToken ct);
}
