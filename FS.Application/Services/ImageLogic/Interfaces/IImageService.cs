using FS.Application.DTOs.ImageDTOs;
using FS.Core.Entities;

namespace FS.Application.Services.ImageLogic.Interfaces;

public interface IImageService
{
    Task<Image> CreateImageAsync(byte[] content, CancellationToken ct, string? fileName = null);

    Task<ImageResponseInfo> DownloadFileAsync(string key, CancellationToken ct);
    
    Task DeleteImageAsync(Guid id, string imagePath, CancellationToken ct);
}
