using FS.Core.Entities;

namespace FS.Application.Services.ImageLogic.Interfaces;

public interface IImageService
{
    Task<Image> CreateImageAsync(byte[] content, string? fileName = null);
}
