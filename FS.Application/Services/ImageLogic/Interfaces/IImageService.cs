using FS.Core.Entities;

namespace FS.Application.Services.ImageLogic.Interfaces;

public interface IImageService
{
    Task<Image> CreateImageAsync(byte[] content, CancellationToken ct, string? fileName = null);
}
