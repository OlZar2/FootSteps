using FS.Application.DTOs.MissingAnnouncementDTOs;

namespace FS.Application.Services.MissingPetLogic.Interfaces;

public interface IMissingAnnouncementService
{
    Task<MissingAnnouncementFeed[]> GetFilteredMissingAnnouncementsByPageAsync(DateTime lastDateTime,
        AnnouncementFilter announcementFilter, CancellationToken ct);

    Task<CreatedMissingAnnouncement> Create(CreateMissingAnnouncementData data, CancellationToken ct);

    Task<MissingAnnouncementPageData> GetForPageByIdAsync(Guid id, CancellationToken ct);

    Task Delete(DeleteMissingAnnouncementData data, CancellationToken ct);
}