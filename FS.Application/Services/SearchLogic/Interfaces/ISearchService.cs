using FS.Application.DTOs.SearchDTOs;

namespace FS.Application.Services.SearchLogic.Interfaces;

public interface ISearchService
{
    Task DoSearch(Guid searchId, CancellationToken ct);

    Task<SearchResultDto[]> GetPaginatedSearchResults(Guid userId, DateTime lastDateTime, CancellationToken ct);

    Task<SearchResultDto> GetSearchResultBySearchRequestId(
        Guid searchRequestId,
        Guid currentUserId,
        CancellationToken ct);

    Task RequestSearchAsync(SearchRequestDto searchRequestDto, CancellationToken ct);
}