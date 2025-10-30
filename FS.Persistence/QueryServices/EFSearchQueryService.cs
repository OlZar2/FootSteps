using FS.Application.DTOs.SearchDTOs;
using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Core.Entities;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Pgvector;

namespace FS.Persistence.QueryServices;

public class EFSearchQueryService(ApplicationDbContext context) : ISearchQueryService
{
    public async Task<List<AnimalAnnouncement>> DoVectorSearch(Guid searchId, CancellationToken ct)
    {
        var search = await context.SearchRequests
                         .FirstOrDefaultAsync(s => s.Id == searchId, cancellationToken: ct)
            ?? throw new NotFoundException(nameof(SimilarAnnouncement), searchId);

        var similarImages = await context.Images
            .FromSqlRaw(@"
                SELECT ""Id"", ""Path""
                FROM ""Images""
                ORDER BY ""Embedding"" <=> @embedding
                LIMIT 5",
                new NpgsqlParameter("embedding", search.Embedding))
            .Select(i => new { i.Id, i.Path })
            .ToArrayAsync(cancellationToken: ct);

        var similarIds = similarImages.Select(i => i.Id).ToArray();
        
        var announcements = await context.AnimalAnnouncements
            .Where(aa => aa.Images.Any(img => similarIds.Contains(img.Id)))
            .ToListAsync(cancellationToken: ct);

        return announcements;
    }

    public async Task<SearchResultDto[]> GetSearchResults(
        Guid userId,
        DateTime lastSearchCreatedAt,
        CancellationToken ct)
    {
        var results = await context.SearchRequests
            .Where(sr => sr.CreatorId == userId && sr.CreatedAt <= lastSearchCreatedAt)
            .OrderBy(sr => sr.CreatedAt)
            .Take(3)
            .Select(sr => new SearchResultDto
            {
                Results = sr.Results.Select(aa => new SimilarAnnouncement
                {
                    Id = aa.Id,
                    MainImagePath = aa.Images[0].Path,
                    District = aa.District,
                    House = aa.House,
                    Street = aa.Street,
                    Type = aa.Type,
                    Breed    = aa is PetAnnouncement
                        ? ((PetAnnouncement)aa).Breed
                        : null,
                }).ToArray(),
                SearchImagePath = sr.ImagePath,
                CreatedAt = sr.CreatedAt,
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
                    MainImagePath = aa.Images[0].Path,
                    District = aa.District,
                    House = aa.House,
                    Street = aa.Street,
                    Type = aa.Type,
                    Breed    = aa is PetAnnouncement
                        ? ((PetAnnouncement)aa).Breed
                        : null
                }).ToArray(),
                SearchImagePath = sr.ImagePath,
                CreatedAt = sr.CreatedAt,
            })
            .FirstOrDefaultAsync(cancellationToken: ct)
            ?? throw new NotFoundException(nameof(SearchRequest), searchRequestId);
        
        return result;
    }
}