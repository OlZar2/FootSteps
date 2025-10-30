using FS.Core.Entities;

namespace FS.Core.Stores;

public interface ISearchRequestRepository
{
    Task AddAsync(SearchRequest searchRequest, CancellationToken ct);
    
    Task<SearchRequest> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task UpdateAsync(SearchRequest image, CancellationToken ct);
}