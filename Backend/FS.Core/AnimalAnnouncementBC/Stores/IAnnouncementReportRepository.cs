using FS.Core.AnimalAnnouncementBC.Entities;

namespace FS.Core.AnimalAnnouncementBC.Stores;

public interface IAnnouncementReportRepository
{
    Task<bool> ExistsAsync(Guid announcementId, Guid reporterId, CancellationToken ct);

    Task CreateAsync(AnnouncementReport report, CancellationToken ct);
}
