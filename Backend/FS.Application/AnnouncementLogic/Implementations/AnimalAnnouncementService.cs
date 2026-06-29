using FS.Application.AnnouncementLogic.DTOs;
using FS.Application.AnnouncementLogic.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Core.AnimalAnnouncementBC.Stores;
using Pgvector;

namespace FS.Application.AnnouncementLogic.Implementations;

public class AnimalAnnouncementService(
    IAnimalAnnouncementRepository animalAnnouncementRepository,
    IAnimalAnnouncementQueryService animalAnnouncementQueryService) : IAnimalAnnouncementService
{
    public async Task UpdateEmbeddingAsync(Guid imageId, Vector vector, CancellationToken ct)
    {
        var announcement = await animalAnnouncementRepository.GetByImageIdAsync(imageId, ct);
        announcement.UpdateImageEmbedding(imageId, vector);

        await animalAnnouncementRepository.SaveChangesAsync(ct);
    }

    public async Task HideByAdminAsync(Guid announcementId, CancellationToken ct)
    {
        var announcement = await animalAnnouncementRepository.GetByIdAsync(announcementId, ct);
        announcement.HideByAdmin();

        await animalAnnouncementRepository.SaveChangesAsync(ct);
    }

    public async Task<AdminAnimalAnnouncementListPage> GetAdminListAsync(
        AdminAnimalAnnouncementListQuery query,
        CancellationToken ct)
    {
        return await animalAnnouncementQueryService.GetAdminListAsync(query, ct);
    }
}
