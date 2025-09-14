using FS.Application.DTOs.FindAnnouncementDTOs;
using FS.Application.DTOs.Shared;

namespace FS.Application.Services.FindAnnouncementLogic.Interfaces;

public interface IFindAnnouncementService
{
    Task<CreatedFindAnnouncement> Create(CreateFindAnnouncementData data, CancellationToken ct);

    Task<FindAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime,
        AnnouncementFilter announcementFilter, CancellationToken ct);

    Task<FindAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);

    Task Cancel(DeleteFindAnnouncementData data, CancellationToken ct);
}