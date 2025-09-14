using FS.Core.Entities;
using FS.Core.Specifications;

namespace FS.Core.Stores;

public interface IMissingAnnouncementRepository
{
    Task<MissingAnnouncement[]> GetFilteredByPageAsync(DateTime lastDateTime,
        PetAnnouncementFeedSpecification<MissingAnnouncement> spec, CancellationToken ct);

    Task CreateAsync(MissingAnnouncement missingAnnouncement, CancellationToken ct);
    
    Task<MissingAnnouncement> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task UpdateAsync(MissingAnnouncement missingAnnouncement, CancellationToken ct);
}