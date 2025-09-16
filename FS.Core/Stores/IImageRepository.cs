using FS.Core.Entities;

namespace FS.Core.Stores;

public interface IImageRepository
{
    Task AddAsync(Image image, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}