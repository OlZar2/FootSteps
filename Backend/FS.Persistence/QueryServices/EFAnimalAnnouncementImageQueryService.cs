using FS.Application.Exceptions;
using FS.Application.Interfaces.QueryServices;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFAnimalAnnouncementImageQueryService(ApplicationDbContext context) : IImageQueryService
{
    public async Task<string> GetStorageKeyByImageId(Guid imageId, CancellationToken ct)
    {
        return await context.Images
            .Where(i => i.Id == imageId)
            .Select(i => i.S3Key)
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException("Image", nameof(imageId));
    }
}