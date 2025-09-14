using FS.Core.Entities;
using FS.Core.Stores;
using FS.Persistence.Context;

namespace FS.Persistence.Repositories;

//TODO: это не агрегат?
public class ImageRepository(ApplicationDbContext context) : IImageRepository
{
    public async Task AddImageAsync(Image image, CancellationToken ct)
    {
        context.Images.Add(image);
        await context.SaveChangesAsync(ct);
    }
}