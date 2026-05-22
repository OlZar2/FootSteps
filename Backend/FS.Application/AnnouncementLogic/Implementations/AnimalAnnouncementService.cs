using FS.Application.AnnouncementLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC.Stores;
using Pgvector;

namespace FS.Application.AnnouncementLogic.Implementations;

public class AnimalAnnouncementService(IAnimalAnnouncementRepository animalAnnouncementRepository) : IAnimalAnnouncementService
{
    public async Task UpdateEmbeddingAsync(Guid imageId, Vector vector, CancellationToken ct)
    {
        var announcement = await animalAnnouncementRepository.GetByImageIdAsync(imageId, ct);
        announcement.UpdateImageEmbedding(imageId, vector);

        await animalAnnouncementRepository.SaveChangesAsync(ct);
    }
}