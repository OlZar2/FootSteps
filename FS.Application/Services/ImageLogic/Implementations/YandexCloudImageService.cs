using FS.Application.Services.ImageLogic.Configurations;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Core.Entities;
using Amazon.S3;
using Amazon.S3.Model;
using FS.Application.Services.ImageLogic.Exceptions;
using FS.Contracts.Error;
using FS.Core.Stores;
using ImageMagick;
using Microsoft.Extensions.Options;

namespace FS.Application.Services.ImageLogic.Implementations;

public class YandexCloudImageService : IImageService
{
    private readonly AmazonS3Client _s3Client;
    private readonly string _bucketName;
    private readonly IImageRepository _imageRepository;
    
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".gif",
        ".webp",
        ".avif",
        ".heic",
        ".heif"
    };

    public YandexCloudImageService(IOptions<S3StorageConfiguration> options, IImageRepository imageRepository)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = options.Value.ServiceURL,
            ForcePathStyle = true,
            AuthenticationRegion = options.Value.Region
        };

        _s3Client = new AmazonS3Client(
            options.Value.AccessKey,
            options.Value.SecretKey,
            config
        );

        _bucketName = options.Value.BucketName;

        _imageRepository = imageRepository;
    }

    public async Task<Image> CreateImageAsync(byte[] content, string? imageName = null)
    {
        var (ext, mime) = DetectImage(content);
        if (!AllowedImageExtensions.Contains(ext))
            throw new ImageValidationException(
                IssueCodes.File.UnsupportedFormat,
                "Поддерживаются только JPG и PNG.",
                imageName);
        
        var fileName = Guid.NewGuid() + ext;

        using MemoryStream memoryStream = new(content);

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = memoryStream,
            ContentType = mime,
        };
        
        var image = Image.Create(fileName);

        await _s3Client.PutObjectAsync(request);
        await _imageRepository.AddImageAsync(image);

        return image;
    }

    private static (string ExtWithDot, string Mime) DetectImage(byte[] content, string? imageName = null)
    {
        if (content == null || content.Length == 0)
            throw new ImageValidationException(IssueCodes.File.Empty, "Файл пустой.", imageName);

        try
        {
            var info = new MagickImageInfo(content);
            var fmt  = info.Format;
            var fi   = MagickFormatInfo.Create(fmt);

            if (fi == null)
                throw new InvalidOperationException("Невозможно определить формат изображения.");

            var ext  = "." + fmt.ToString().ToLowerInvariant();
            var mime = fi.MimeType ?? "application/octet-stream";

            return (ext, mime);
        }
        catch (MagickCorruptImageErrorException)
        {
            throw new ImageValidationException(
                IssueCodes.File.NotImageOrCorrupt,
                "Файл повреждён или не является изображением.",
                imageName);
        }
    }
}