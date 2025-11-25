using FS.Core.Entities;
using FS.Core.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class StreetPetAnnouncementRepository(ApplicationDbContext context) : IStreetPetAnnouncementRepository
{
    public async Task CreateAsync(StreetPetAnnouncement missingAnnouncement, CancellationToken ct)
    {
        context.StreetPetAnnouncements.Add(missingAnnouncement);
        await context.SaveChangesAsync(ct);
    }

    public async Task<StreetPetAnnouncement?> GetByImageIdAsync(Guid imageId, CancellationToken ct)
    {
        var streetPetAnnouncement = await context.StreetPetAnnouncements
            .Where(ma => ma.Images.Any(i => i.Id == imageId))
            .FirstOrDefaultAsync(ct);
        
        return streetPetAnnouncement;
    }
}