namespace FS.Application.Services.ImageLogic.Interfaces;

public interface IImageService
{
    Task<Guid> UploadAsync(Stream stream, CancellationToken ct);
}