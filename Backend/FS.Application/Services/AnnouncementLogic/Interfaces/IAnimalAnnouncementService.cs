using Pgvector;

namespace FS.Application.Services.AnnouncementLogic.Interfaces;

public interface IAnimalAnnouncementService
{
    Task UpdateEmbeddingAsync(Guid imageId, Vector vector, CancellationToken ct);
}