using FS.Application.AnnouncementLogic.Configurations;
using FS.Application.AnnouncementLogic.DTOs;
using FS.Application.AnnouncementLogic.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Core.AnimalAnnouncementBC.Stores;
using Microsoft.Extensions.Options;
using Pgvector;

namespace FS.Application.AnnouncementLogic.Implementations;

public class AnimalAnnouncementService(
    IAnimalAnnouncementRepository animalAnnouncementRepository,
    IAnimalAnnouncementQueryService animalAnnouncementQueryService,
    IOptions<AdminAnimalAnnouncementListOptions> adminAnimalAnnouncementListOptions) : IAnimalAnnouncementService
{
    public async Task UpdateEmbeddingAsync(Guid imageId, Vector vector, CancellationToken ct)
    {
        var announcement = await animalAnnouncementRepository.GetByImageIdWithImagesAsync(imageId, ct);
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
        var options = adminAnimalAnnouncementListOptions.Value;
        var normalizedQuery = new AdminAnimalAnnouncementListQuery
        {
            PageSize = query.PageSize ?? options.DefaultPageSize,
            Cursor = query.Cursor,
            CreatedAtFrom = query.CreatedAtFrom,
            CreatedAtTo = query.CreatedAtTo,
            ReportsCountFrom = query.ReportsCountFrom,
            ReportsCountTo = query.ReportsCountTo,
            SortBy = query.SortBy,
            SortDirection = query.SortDirection,
        };

        return await animalAnnouncementQueryService.GetAdminListAsync(normalizedQuery, ct);
    }
}
