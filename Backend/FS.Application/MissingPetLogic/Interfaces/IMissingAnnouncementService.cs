using FS.Application.MissingPetLogic.DTOs;
using FS.Application.Shared.DTOs;

namespace FS.Application.MissingPetLogic.Interfaces;

public interface IMissingAnnouncementService
{
    Task<MissingAnnouncementFeed[]> GetFeedAsync(
        AnnouncementFilter announcementFilter,
        DateTime? lastDateTime = null,
        CancellationToken ct = default);

    Task Create(CreateMissingAnnouncementData data, CancellationToken ct);

    Task<MissingAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);

    Task Cancel(DeleteMissingAnnouncementData data, CancellationToken ct);

    Task<MyAnnouncementFeed[]> GetFeedItemsByCreatorByPage(
        Guid creatorId,
        DateTime? lastDateTime = null,
        CancellationToken ct = default);

    Task ReportSpottedAsync(SpottedInfo spottedInfo, CancellationToken ct);

    Task ReportFoundAsync(FoundInfo foundInfo, CancellationToken ct);

    Task<SpottedLocationDto[]> GetSpottedLocations(Guid missingAnnouncementId, CancellationToken ct);

    Task<FoundReportDto[]> GetFoundReports(Guid missingAnnouncementId, CancellationToken ct);
}