using FS.Application.Shared.Exceptions;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class EFAnimalAnnouncementRepository(ApplicationDbContext context) : IAnimalAnnouncementRepository
{
    public async Task<AnimalAnnouncement> GetByImageIdAsync(Guid imageId, CancellationToken ct)
    {
        return await context.AnimalAnnouncements
            .Where(aa => aa.Images.Select(i => i.Id).Contains(imageId))
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException($"announcement by image {imageId} not found");
    }

    public async Task SaveChangesAsync(CancellationToken ct)
    {
        await context.SaveChangesAsync(ct);
    }
}