namespace FS.Core.AnimalAnnouncementBC.Stores;

public interface IAnimalAnnouncementRepository
{
    Task<AnimalAnnouncement> GetByImageIdAsync(Guid imageId, CancellationToken ct);

    Task<AnimalAnnouncement> GetByIdAsync(Guid id, CancellationToken ct);

    Task<bool> ExistsByIdAsync(Guid id, CancellationToken ct);
    
    Task SaveChangesAsync(CancellationToken ct);
}
