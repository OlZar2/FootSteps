using Amazon.S3;
using Amazon.S3.Model;
using FS.Application.Interfaces.Storages;
using FS.Application.Services.ImageLogic.Configurations;
using FS.Application.Services.ImageLogic.Exceptions;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Contracts.Error;
using ImageMagick;
using Microsoft.Extensions.Options;

namespace FS.S3;

public class YandexCloudImageStorageService : IImageStorageService
{
    private readonly AmazonS3Client _s3Client;
    private readonly string _bucketName;
    private readonly ImagesOptions _imagesOptions;

    public YandexCloudImageStorageService(
        IOptions<S3StorageConfiguration> s3Options,
        IOptions<ImagesOptions> imagesOptions)
    {
        var config = new AmazonS3Config
        {
            ServiceURL = s3Options.Value.ServiceURL,
            ForcePathStyle = true,
            AuthenticationRegion = s3Options.Value.Region
        };

        _s3Client = new AmazonS3Client(
            s3Options.Value.AccessKey,
            s3Options.Value.SecretKey,
            config
        );

        _bucketName = s3Options.Value.BucketName;
        _imagesOptions = imagesOptions.Value;
    }
    
    public async Task UploadAsync(
        Stream stream,
        string storageKey,
        CancellationToken ct)
    {
        var (ext, mime) = DetectImage(stream);
        if (!_imagesOptions.AllowedImageExtensions.Contains(ext))
            throw new ImageValidationException(
                IssueCodes.File.UnsupportedFormat,
                "Поддерживаются только JPG и PNG.");
        
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = storageKey,
            InputStream = stream,
            ContentType = mime,
        };
        
        await _s3Client.PutObjectAsync(request, ct);
    }

    public async Task DeleteAsync(string storageKey, CancellationToken ct)
    {
        var request = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = storageKey
        };

        await _s3Client.DeleteObjectAsync(request, ct);
    }

    private static (string ExtWithDot, string Mime) DetectImage(Stream stream)
    {
        if (stream == null)
            throw new ImageValidationException(IssueCodes.File.Empty, "Файл пустой.");

        try
        {
            var info = new MagickImageInfo(stream);
            var fmt  = info.Format;
            var fi   = MagickFormatInfo.Create(fmt);

            if (fi == null)
                throw new InvalidOperationException("Невозможно определить формат изображения.");

            var ext  = fmt.ToString().ToLowerInvariant();
            var mime = fi.MimeType ?? "application/octet-stream";

            return (ext, mime);
        }
        catch (MagickCorruptImageErrorException)
        {
            throw new ImageValidationException(
                IssueCodes.File.NotImageOrCorrupt,
                "Файл повреждён или не является изображением.");
        }
    }
}