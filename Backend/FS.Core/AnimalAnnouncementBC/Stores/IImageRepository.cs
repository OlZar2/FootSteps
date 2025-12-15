using FS.Core.ImageDomain.Entities;

namespace FS.Core.AnimalAnnouncementBC.Stores;

public interface IImageRepository
{
    Task AddAsync(FSImage fsImage, CancellationToken ct);
    
    Task<FSImage[]> GetByIdsAsync(Guid[] ids, CancellationToken ct);
    
    Task<FSImage> GetByIdAsync(Guid id, CancellationToken ct);
}