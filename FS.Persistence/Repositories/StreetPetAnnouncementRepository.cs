using FS.Core.Entities;
using FS.Core.Stores;
using FS.Persistence.Context;

namespace FS.Persistence.Repositories;

public class StreetPetAnnouncementRepository(ApplicationDbContext context) : IStreetPetAnnouncementRepository
{
    public async Task CreateAsync(StreetPetAnnouncement missingAnnouncement, CancellationToken ct)
    {
        context.StreetPetAnnouncements.Add(missingAnnouncement);
        await context.SaveChangesAsync(ct);
    }
}