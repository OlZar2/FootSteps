using FS.Application.SearchLogic.DTOs;

namespace FS.Application.SearchLogic.Interfaces;

public interface ISearchService
{
    Task DoSearch(Guid searchId, CancellationToken ct);

    Task<SearchResultDto[]> GetPaginatedSearchResults(Guid userId, DateTime lastDateTime, CancellationToken ct);

    Task<SearchResultDto> GetSearchResultBySearchRequestId(
        Guid searchRequestId,
        Guid currentUserId,
        CancellationToken ct);

    Task RequestSearchAsync(SearchRequestDto searchRequestDto, CancellationToken ct);

    Task SetSearchEmbeddingAsync(Guid searchId, float[] vector, CancellationToken ct);
}