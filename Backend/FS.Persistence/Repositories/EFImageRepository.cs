using FS.Application.Shared.Exceptions;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.ImageDomain.Entities;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

public class EFImageRepository(ApplicationDbContext context) : IImageRepository
{
    public async Task AddAsync(FSImage fsImage, CancellationToken ct)
    {
        context.Images.Add(fsImage);
        await context.SaveChangesAsync(ct);
    }

    public async Task<FSImage[]> GetByIdsAsync(Guid[] ids, CancellationToken ct)
    {
        return await context.Images.Where(x => ids.Contains(x.Id)).ToArrayAsync(ct);
    }

    public async Task<FSImage> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Images.FirstOrDefaultAsync(x => x.Id == id, ct)
            ?? throw new NotFoundException("Image", id);
    }
}