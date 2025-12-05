
namespace FS.Application.Services.ImageLogic.Interfaces;

public interface IImageStorageService
{
    Task UploadAsync(
        byte[] content,
        string storageKey,
        CancellationToken ct);

    Task DeleteAsync(string storageKey, CancellationToken ct);
}
