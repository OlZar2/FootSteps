using FS.Application.Interfaces.Storages;
using FS.Application.Services.ImageLogic.Configurations;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.ImageDomain.Entities;
using Microsoft.Extensions.Options;

namespace FS.Application.Services.ImageLogic.Implementations;

public class ImageService(
    IImageStorageService imageStorageService,
    IImageRepository imageRepository,
    IOptions<S3StorageConfiguration> s3StorageConfigurationOptions) : IImageService
{
    private readonly S3StorageConfiguration _s3StorageConfiguration = s3StorageConfigurationOptions.Value;
    
    public async Task<Guid> UploadAsync(Stream stream, CancellationToken ct)
    {
        var storageKey = Guid.NewGuid().ToString();
        var image = FSImage.Create(storageKey, _s3StorageConfiguration.ImagesBucketUrl);
        
        await imageStorageService.UploadAsync(stream, storageKey, ct);
        await imageRepository.AddAsync(image, ct);
        
        return image.Id;
    }
}