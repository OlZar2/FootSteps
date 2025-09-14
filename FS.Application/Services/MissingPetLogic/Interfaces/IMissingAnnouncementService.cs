using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;

namespace FS.Application.Services.MissingPetLogic.Interfaces;

public interface IMissingAnnouncementService
{
    Task<MissingAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime,
        AnnouncementFilter announcementFilter, CancellationToken ct);

    Task<CreatedMissingAnnouncement> Create(CreateMissingAnnouncementData data, CancellationToken ct);

    Task<MissingAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);

    Task Delete(DeleteMissingAnnouncementData data, CancellationToken ct);
}