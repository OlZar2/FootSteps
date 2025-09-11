using FS.Core.Entities;
using FS.Core.Stores;
using FS.Persistence.Context;

namespace FS.Persistence.Repositories;

public class ImageRepository(ApplicationDbContext context) : IImageRepository
{
    public async Task AddImageAsync(Image image)
    {
        context.Images.Add(image);
        await context.SaveChangesAsync();
    }
}