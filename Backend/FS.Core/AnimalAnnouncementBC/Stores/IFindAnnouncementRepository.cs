namespace FS.Core.AnimalAnnouncementBC.Stores;

public interface IFindAnnouncementRepository
{
    Task CreateAsync(FindAnnouncement findAnnouncement, CancellationToken ct);
    
    Task<FindAnnouncement> GetByIdAsync(Guid id, CancellationToken ct);

    Task UpdateAsync(FindAnnouncement findAnnouncement, CancellationToken ct);
}