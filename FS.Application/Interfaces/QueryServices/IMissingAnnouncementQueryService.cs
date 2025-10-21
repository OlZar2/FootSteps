using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;

namespace FS.Application.Interfaces.QueryServices;

public interface IMissingAnnouncementQueryService
{
    Task<MissingAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);

    Task<MyAnnouncementFeed[]> GetFeedForUserAsync(Guid id, DateTime lastDateTime, CancellationToken ct);
}