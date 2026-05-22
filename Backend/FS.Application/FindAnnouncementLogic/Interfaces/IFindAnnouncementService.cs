using FS.Application.FindAnnouncementLogic.DTOs;
using FS.Application.Shared.DTOs;

namespace FS.Application.FindAnnouncementLogic.Interfaces;

public interface IFindAnnouncementService
{
    Task Create(CreateFindAnnouncementData data, CancellationToken ct);

    Task<FindAnnouncementFeed[]> GetFeedAsync(
        AnnouncementFilter announcementFilter,
        DateTime? lastDateTime = null,
        CancellationToken ct = default);

    Task<FindAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);

    Task Cancel(DeleteFindAnnouncementData data, CancellationToken ct);

    Task<MyAnnouncementFeed[]> GetFeedItemsByCreatorByPage(
        Guid userId,
        DateTime? lastDateTime = null,
        CancellationToken ct = default);
}