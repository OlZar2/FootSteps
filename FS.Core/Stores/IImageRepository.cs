using FS.Core.Entities;

namespace FS.Core.Stores;

public interface IImageRepository
{
    Task AddImageAsync(Image image);
}