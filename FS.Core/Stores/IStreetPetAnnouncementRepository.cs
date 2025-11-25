using FS.Core.Entities;

namespace FS.Core.Stores;

public interface IStreetPetAnnouncementRepository
{
    Task CreateAsync(StreetPetAnnouncement missingAnnouncement, CancellationToken ct);

    Task<StreetPetAnnouncement?> GetByImageIdAsync(Guid imageId, CancellationToken ct);
}