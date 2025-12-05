using Pgvector;

namespace FS.Core.AnimalAnnouncementBC.Stores;

public interface IMissingAnnouncementRepository
{
    Task CreateAsync(MissingAnnouncement missingAnnouncement, CancellationToken ct);
    
    Task<MissingAnnouncement> GetByIdAsync(Guid id, CancellationToken ct);
    
    Task UpdateAsync(MissingAnnouncement missingAnnouncement, CancellationToken ct);

     Task<MissingAnnouncement[]> GetSimilarMissingAnnouncementAsync(Vector vector, CancellationToken ct);
}