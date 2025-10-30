using FS.Application.DTOs.SearchDTOs;
using FS.Core.Entities;

namespace FS.Application.Interfaces.QueryServices;

public interface ISearchQueryService
{
    Task<List<AnimalAnnouncement>> DoVectorSearch(Guid searchId, CancellationToken ct);

    Task<SearchResultDto[]> GetSearchResults(
        Guid userId,
        DateTime lastSearchCreatedAt,
        CancellationToken ct);

    Task<SearchResultDto> GetSearchResultsBySearchRequestId(
        Guid searchRequestId,
        CancellationToken ct);
}