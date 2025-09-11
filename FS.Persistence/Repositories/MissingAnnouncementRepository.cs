using FS.Core.Entities;
using FS.Core.Specifications;
using FS.Core.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class MissingAnnouncementRepository(ApplicationDbContext context) : IMissingAnnouncementRepository
{
    public async Task<MissingAnnouncement[]> GetFilteredMissingAnnouncementByPageAsync(DateTime lastDateTime, 
        MissingAnnouncementSpecification spec)
    {
        return await context.MissingAnnouncements
            .OrderByDescending(ma => ma.CreatedAt)
            .Where(spec.Criteria)
            .Where(ma => ma.CreatedAt > lastDateTime)
            .Take(20)
            .ToArrayAsync();
    }
}