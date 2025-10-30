using FS.Application.DTOs.ImageDTOs;
using FS.Core.Entities;

namespace FS.Application.Services.ImageLogic.Interfaces;

public interface IImageService
{
    Task<Image> CreateImageForAnnouncementAsync(
        byte[] content,
        CancellationToken ct,
        string? imageName = null);
    
    Task<Image> CreateImageAsync(byte[] content, CancellationToken ct, string? fileName = null);

    Task<ImageResponseInfo> DownloadFileAsync(string key, CancellationToken ct);
    
    Task DeleteImageAsync(Guid id, string imagePath, CancellationToken ct);

    Task<string> PutInS3(byte[] content, CancellationToken ct, string? imageName = null);
}
