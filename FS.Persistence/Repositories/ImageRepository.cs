using FS.Core.Entities;
using FS.Core.Stores;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.Repositories;

//TODO: это не агрегат?
public class ImageRepository(ApplicationDbContext context) : IImageRepository
{
    public async Task AddAsync(Image image, CancellationToken ct)
    {
        context.Images.Add(image);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        await context.Images.Where(i => i.Id == id).ExecuteDeleteAsync(ct);
    }
}