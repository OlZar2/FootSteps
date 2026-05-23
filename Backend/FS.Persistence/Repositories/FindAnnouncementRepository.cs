using FS.Application.Shared.Exceptions;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class FindAnnouncementRepository(ApplicationDbContext context) : IFindAnnouncementRepository
{
    public async Task CreateAsync(FindAnnouncement findAnnouncement, CancellationToken ct)
    {
        context.FindAnnouncements.Add(findAnnouncement);
        await context.SaveChangesAsync(ct);
    }

    public async Task<FindAnnouncement> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var findAnnouncement = await context.FindAnnouncements
                                      .FirstOrDefaultAsync(ms => ms.Id == id, ct)
                                  ?? throw new NotFoundException(nameof(MissingAnnouncement), id);
        
        return findAnnouncement;
    }
    
    public async Task UpdateAsync(FindAnnouncement findAnnouncement, CancellationToken ct)
    {
        context.FindAnnouncements.Update(findAnnouncement);
        await context.SaveChangesAsync(ct);
    }
}