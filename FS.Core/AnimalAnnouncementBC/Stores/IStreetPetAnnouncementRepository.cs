namespace FS.Core.AnimalAnnouncementBC.Stores;

public interface IStreetPetAnnouncementRepository
{
    Task CreateAsync(StreetPetAnnouncement missingAnnouncement, CancellationToken ct);

    Task<StreetPetAnnouncement?> GetByImageIdAsync(Guid imageId, CancellationToken ct);
}