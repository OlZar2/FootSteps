using Pgvector;

namespace FS.Application.AnnouncementLogic.Interfaces;

public interface IAnimalAnnouncementService
{
    Task UpdateEmbeddingAsync(Guid imageId, Vector vector, CancellationToken ct);

    Task HideByAdminAsync(Guid announcementId, CancellationToken ct);
}
