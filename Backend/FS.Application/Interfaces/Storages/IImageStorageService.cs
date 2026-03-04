
namespace FS.Application.Interfaces.Storages;

public interface IImageStorageService
{
    Task UploadAsync(Stream stream, string storageKey, CancellationToken ct);

    Task UploadAsync(byte[] content, string storageKey, CancellationToken ct);

    Task DeleteAsync(string storageKey, CancellationToken ct);
}
