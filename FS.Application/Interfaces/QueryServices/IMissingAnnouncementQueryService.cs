using FS.Application.DTOs.MissingAnnouncementDTOs;

namespace FS.Application.Interfaces.QueryServices;

public interface IMissingAnnouncementQueryService
{
    Task<MissingAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct);
}