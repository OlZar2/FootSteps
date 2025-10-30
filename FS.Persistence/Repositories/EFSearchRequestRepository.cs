using FS.Application.Exceptions;
using FS.Core.Entities;
using FS.Core.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class EFSearchRequestRepository(ApplicationDbContext context) : ISearchRequestRepository
{
    public async Task<SearchRequest> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.SearchRequests.FirstOrDefaultAsync(sr => sr.Id == id, ct) ??
               throw new NotFoundException(nameof(SearchRequest), id);
    }

    public async Task UpdateAsync(SearchRequest image, CancellationToken ct)
    {
        context.SearchRequests.Update(image);
        await context.SaveChangesAsync(ct);
    }

    public async Task AddAsync(SearchRequest searchRequest, CancellationToken ct)
    {
        context.SearchRequests.Add(searchRequest);
        await context.SaveChangesAsync(ct);
    }
}