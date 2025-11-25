using FS.Application.Exceptions;
using FS.Core.Entities;
using FS.Core.Specifications;
using FS.Core.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pgvector;

namespace FS.Persistence.Repositories;

public class MissingAnnouncementRepository(ApplicationDbContext context) : IMissingAnnouncementRepository
{
    //TODO: перенести в queryService
    public async Task<MissingAnnouncement[]> GetFilteredByPageAsync(DateTime lastDateTime, 
        PetAnnouncementFeedSpecification<MissingAnnouncement> spec, CancellationToken ct)
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
    
    public async Task<MissingAnnouncement[]> GetSimilarMissingAnnouncementAsync(Vector vector, CancellationToken ct)
    {
        var embeddingParam = new NpgsqlParameter("embedding", vector);
        var maxDistanceParam = new NpgsqlParameter("maxDistance", 0.1f);
        
        var similarAnnouncements = await context.MissingAnnouncements
            .FromSqlRaw(@"
                WITH ranked AS MATERIALIZED (
                    SELECT
                        i.""AnimalAnnouncementId"",
                        i.""Embedding"" <=> @embedding AS dist,
                        ROW_NUMBER() OVER (
                            PARTITION BY i.""AnimalAnnouncementId""
                            ORDER BY i.""Embedding"" <=> @embedding
                        ) AS rn
                    FROM ""Images"" i
                    JOIN ""AnimalAnnouncements"" a ON a.""Id"" = i.""AnimalAnnouncementId""
                    WHERE a.""Type"" = 1
                      AND (i.""Embedding"" <=> @embedding) <= @maxDistance
                )
                SELECT a.*
                FROM ranked r
                JOIN ""AnimalAnnouncements"" a ON a.""Id"" = r.""AnimalAnnouncementId""
                WHERE r.rn = 1
                ORDER BY r.dist
                LIMIT 30",
                embeddingParam, maxDistanceParam)
            .ToArrayAsync(ct);

        return similarAnnouncements;
    }
}