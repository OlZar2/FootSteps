using FS.Application.Exceptions;
using FS.Core.Entities;
using FS.Core.Specifications;
using FS.Core.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class MissingAnnouncementRepository(ApplicationDbContext context) : IMissingAnnouncementRepository
{
    //TODO: перенести в queryService
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

    public async Task<MissingAnnouncement> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var missingAnnouncement = await context.MissingAnnouncements
            .FirstOrDefaultAsync(ms => ms.Id == id, ct)
        ?? throw new NotFoundException(nameof(MissingAnnouncement), id);
        
        return missingAnnouncement;
    }

    public async Task UpdateAsync(MissingAnnouncement missingAnnouncement, CancellationToken ct)
    {
        context.MissingAnnouncements.Update(missingAnnouncement);
        await context.SaveChangesAsync(ct);
    }
}