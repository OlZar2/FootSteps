using FS.Application.Shared.Exceptions;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class EFAnimalAnnouncementRepository(ApplicationDbContext context) : IAnimalAnnouncementRepository
{
    public async Task<AnimalAnnouncement> GetByImageIdWithImagesAsync(Guid imageId, CancellationToken ct)
    {
        return await context.AnimalAnnouncements
            .Include(aa => aa.Images)
            .Where(aa => aa.Images.Select(i => i.Id).Contains(imageId))
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException($"announcement by image {imageId} not found");
    }

    public async Task<AnimalAnnouncement> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.AnimalAnnouncements
            .FirstOrDefaultAsync(aa => aa.Id == id, ct)
            ?? throw new NotFoundException(nameof(AnimalAnnouncement), id);
    }

    public async Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.AnimalAnnouncements
            .AnyAsync(aa => aa.Id == id, ct);
    }

    public async Task IncrementReportCountAsync(Guid id, CancellationToken ct)
    {
        await context.AnimalAnnouncements
            .Where(aa => aa.Id == id)
            .ExecuteUpdateAsync(
                updates => updates.SetProperty(
                    aa => aa.ReportCount,
                    aa => aa.ReportCount + 1),
                ct);
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await context.SaveChangesAsync(ct);
    }
}
