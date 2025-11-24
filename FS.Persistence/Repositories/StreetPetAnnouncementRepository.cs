using FS.Core.Entities;
using FS.Core.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pgvector;

namespace FS.Persistence.Repositories;

public class StreetPetAnnouncementRepository(ApplicationDbContext context) : IStreetPetAnnouncementRepository
{
    public async Task CreateAsync(StreetPetAnnouncement missingAnnouncement, CancellationToken ct)
    {
        context.StreetPetAnnouncements.Add(missingAnnouncement);
        await context.SaveChangesAsync(ct);
    }

    public async Task<StreetPetAnnouncement[]> GetSimilarStreetPets(Vector vector, CancellationToken ct)
    {
        var embeddingParam = new NpgsqlParameter("embedding", vector);
        var maxDistanceParam = new NpgsqlParameter("maxDistance", 0.1f);
        
        var similarAnnouncements = await context.StreetPetAnnouncements
            .FromSqlRaw(@"
                WITH ranked AS MATERIALIZED (
                    SELECT
                        i.""AnimalAnnouncementId"",
                        i.""Embedding"" <=> @embedding AS dist,
                        ROW_NUMBER() OVER (
                            PARTITION BY i.""AdId""
                            ORDER BY i.""Embedding"" <=> @embedding
                        ) AS rn
                    FROM ""Images"" i
                    JOIN ""Ads"" a ON a.""Id"" = i.""AdId""
                    WHERE a.""Type"" = 2
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