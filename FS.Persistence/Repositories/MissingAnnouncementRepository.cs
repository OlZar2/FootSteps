using FS.Application.Exceptions;
using FS.Core.Entities;
using FS.Core.Specifications;
using FS.Core.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class MissingAnnouncementRepository(ApplicationDbContext context) : IMissingAnnouncementRepository
{
    public async Task<MissingAnnouncement[]> GetFilteredByPageAsync(DateTime lastDateTime, 
        MissingAnnouncementSpecification spec, CancellationToken ct)
    {
        IQueryable<MissingAnnouncement> query = context.MissingAnnouncements;
        
        foreach (var include in spec.Includes) query = query.Include(include);
        
        return await query
            .OrderByDescending(ma => ma.CreatedAt)
            .Where(spec.Criteria)
            .Where(ma => ma.CreatedAt > lastDateTime)
            .Take(20)
            .AsNoTracking()
            .ToArrayAsync(ct);
    }

    public async Task CreateAsync(MissingAnnouncement missingAnnouncement, CancellationToken ct)
    {
        context.MissingAnnouncements.Add(missingAnnouncement);
        await context.SaveChangesAsync(ct);
    }

    public async Task<MissingAnnouncement> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        var missingAnnouncement = await context.MissingAnnouncements
            .Include(ma => ma.Images)
            .Include(ma => ma.Creator)
            .ThenInclude(ma => ma.AvatarImage)
            .FirstOrDefaultAsync(ms => ms.Id == id, ct)
        ?? throw new NotFoundException(nameof(MissingAnnouncement), id);
        
        return missingAnnouncement;
    }
}