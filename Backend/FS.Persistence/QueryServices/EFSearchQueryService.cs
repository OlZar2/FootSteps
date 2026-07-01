using FS.Application.Interfaces.QueryServices;
using FS.Application.SearchLogic.DTOs;
using FS.Application.Shared.Exceptions;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.SearchDomain;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace FS.Persistence.QueryServices;

public class EFSearchQueryService(ApplicationDbContext context) : ISearchQueryService
{
    public async Task<List<AnimalAnnouncement>> DoVectorSearch(Guid searchId, CancellationToken ct)
    {
        var search = await context.SearchRequests
                         .FirstOrDefaultAsync(s => s.Id == searchId, cancellationToken: ct)
            ?? throw new NotFoundException(nameof(SimilarAnnouncement), searchId);
        
        var excludeType = 1;
        var radiusMeters = 10000;
        var latitude = search.Location.Y;
        var longitude = search.Location.X;
        var minSimilarity = 0.75;

        var similarImages = await context.Images
            .FromSqlRaw(@"
                SELECT i.""Id"", i.""FullImagePath"", i.""AnimalAnnouncementId"", i.""Embedding""
                FROM ""Images"" i
                INNER JOIN ""AnimalAnnouncements"" a
                    ON a.""Id"" = i.""AnimalAnnouncementId""
                WHERE a.""Type"" <> @excludeType
                  AND a.""CreatorId"" <> @creatorId
                  AND ST_DWithin(
                      a.""Location"",
                      ST_SetSRID(ST_MakePoint(@longitude, @latitude), 4326)::geography,
                      @radiusMeters
                  )
                  AND 1 - (i.""Embedding"" <=> @embedding) >= @minSimilarity
                ORDER BY i.""Embedding"" <=> @embedding
                LIMIT 5",
                new NpgsqlParameter("excludeType", excludeType),
                new NpgsqlParameter("creatorId", search.CreatorId),
                new NpgsqlParameter("longitude", longitude),
                new NpgsqlParameter("latitude", latitude),
                new NpgsqlParameter("radiusMeters", radiusMeters),
                new NpgsqlParameter("embedding", search.Embedding),
                new NpgsqlParameter("minSimilarity", minSimilarity))
            .Select(i => new
            {
                i.Id,
                i.FullImagePath
            })
            .ToArrayAsync(ct);

        var similarIds = similarImages.Select(i => i.Id).ToArray();
        
        var announcements = await context.AnimalAnnouncements
            .Where(aa => aa.Images.Any(img => similarIds.Contains(img.Id)))
            .ToListAsync(cancellationToken: ct);

        return announcements;
    }

    public async Task<SearchResultDto[]> GetSearchResults(
        Guid userId,
        DateTime? lastSearchCreatedAt,
        CancellationToken ct)
    {
        lastSearchCreatedAt ??= DateTime.MaxValue;
        
        var results = await context.SearchRequests
            .Where(sr => sr.CreatorId == userId && sr.CreatedAt < lastSearchCreatedAt)
            .OrderByDescending(ma => ma.CreatedAt)
            .Take(3)
            .Select(sr => new SearchResultDto
            {
                Results = sr.Results.Select(aa => new SimilarAnnouncement
                {
                    Id = aa.Id,
                    MainImagePath = aa.Images.First().FullImagePath,
                    District = aa.District,
                    House = aa.House,
                    Street = aa.Street,
                    Type = aa.Type,
                    Breed    = aa is PetAnnouncement
                        ? ((PetAnnouncement)aa).Breed
                        : null,
                }).ToArray(),
                SearchImagePath = context.Images
                    .Where(i => i.Id == sr.ImageId)
                    .Select(i => i.FullImagePath)
                    .First(),
                CreatedAt = sr.CreatedAt,
                ErrorCode = sr.ErrorCode,
            })
            .ToArrayAsync(cancellationToken: ct);
        
        return results;
    }
    
    public async Task<SearchResultDto> GetSearchResultsBySearchRequestId(
        Guid searchRequestId,
        CancellationToken ct)
    {
        var result = await context.SearchRequests
            .Where(sr => sr.Id == searchRequestId)
            .OrderBy(sr => sr.CreatedAt)
            .Take(3)
            .Select(sr => new SearchResultDto
            {
                Results = sr.Results.Select(aa => new SimilarAnnouncement
                {
                    Id = aa.Id,
                    MainImagePath = aa.Images.First().FullImagePath,
                    District = aa.District,
                    House = aa.House,
                    Street = aa.Street,
                    Type = aa.Type,
                    Breed    = aa is PetAnnouncement
                        ? ((PetAnnouncement)aa).Breed
                        : null
                }).ToArray(),
                SearchImagePath = context.Images
                    .Where(i => i.Id == sr.ImageId)
                    .Select(i => i.FullImagePath)
                    .First(),
                CreatedAt = sr.CreatedAt,
                ErrorCode = sr.ErrorCode,
            })
            .FirstOrDefaultAsync(cancellationToken: ct)
            ?? throw new NotFoundException(nameof(SearchRequest), searchRequestId);
        
        return result;
    }

    public async Task<Guid[]> GetSearchCreatorDeviceIdsBySearchRequestId(
        Guid searchRequestId,
        CancellationToken ct)
    {
        return await (
            from sr in context.SearchRequests.AsNoTracking()
            join creator in context.Users on sr.CreatorId equals creator.Id
            from device in creator.UserDevices
            where sr.Id == searchRequestId && device.IsActive
            select device.Id
        ).ToArrayAsync(ct);
    }
}