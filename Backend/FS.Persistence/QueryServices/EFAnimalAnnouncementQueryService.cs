using FS.Application.Interfaces.Events;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Shared.Exceptions;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace FS.Persistence.QueryServices;

public class EFAnimalAnnouncementQueryService(ApplicationDbContext context) : IAnimalAnnouncementQueryService
{
    public async Task<DeleteType?> GetDeleteTypeByIdAsync(Guid announcementId, CancellationToken ct)
    {
        var announcement = await context.AnimalAnnouncements
            .Where(aa => aa.Id == announcementId)
            .Select(aa => new { aa.DeleteType })
            .FirstOrDefaultAsync(ct);

        if (announcement is null)
            throw new NotFoundException(nameof(AnimalAnnouncement), announcementId);

        return announcement.DeleteType;
    }

    public async Task<EmbedRequest[]> GetDataForEmbeddingRequestByAnnouncementId(Guid announcementId, CancellationToken ct)
    {
        return await context.AnimalAnnouncements
            .Where(aa => aa.Id == announcementId)
            .Select(aa => aa.Images.Select(i => new EmbedRequest
            {
                ImageId = i.Id,
                ImageUrl = i.FullImagePath,
            }).ToArray())
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException($"images for announcement {announcementId} not found");
    }
}
