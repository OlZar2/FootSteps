using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using FS.Application.DTOs.ImageDTOs;
using FS.Application.Interfaces.Events;
using FS.Application.Interfaces.Transaction;
using FS.Application.Services.ImageLogic.Configurations;
using FS.Application.Services.ImageLogic.Exceptions;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Contracts.Error;
using FS.Core.Entities;
using FS.Core.Enums;
using FS.Core.Stores;
using FS.Persistence.Repositories;
using ImageMagick;
using Microsoft.Extensions.Options;
using Pgvector;

namespace FS.Application.Services.ImageLogic.Implementations;

public class YandexCloudImageService : IImageService
{
    private readonly AmazonS3Client _s3Client;
    private readonly string _bucketName;
    private readonly IImageRepository _imageRepository;
    private readonly IOutboxRepository _outboxRepository;
    private readonly ITransactionFactory _transactionFactory;
    
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
    
    private static readonly Dictionary<string, string> MimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"]  = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"]  = "image/png",
        [".gif"]  = "image/gif",
        [".webp"] = "image/webp",
        [".avif"] = "image/avif",
        [".heic"] = "image/heic",
        [".heif"] = "image/heif"
    };

    public YandexCloudImageService(
        IOptions<S3StorageConfiguration> options,
        IImageRepository imageRepository,
        IOutboxRepository outboxRepository,
        ITransactionFactory transactionFactory)
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
        _outboxRepository = outboxRepository;
        _transactionFactory = transactionFactory;
    }
    
    public async Task<Image> CreateImageForAnnouncementAsync(
        byte[] content,
        AnnouncementType announcementType,
        CancellationToken ct,
        string? imageName = null)
    {
        await using var transaction = await _transactionFactory.BeginAsync(ct);
        
        var image = await CreateImageAsync(content, ct, imageName);
        
        var outboxPayload = JsonSerializer.Serialize(new EmbedRequest{
            ImageId = image.Id,
            ImageUrl = $"http://79.141.79.120:5000/api/image/{image.Path}",
            AnnouncementType = announcementType,
        });
        var outboxEvent = OutboxEvent.Create("image.embed.request", outboxPayload);
        await _outboxRepository.AddAsync(outboxEvent, ct);
        
        await transaction.CommitAsync(ct);

        return image;
    }

    public async Task<Image> CreateImageAsync(byte[] content, CancellationToken ct, string? imageName = null)
    {
        var (ext, mime) = DetectImage(content);
        if (!AllowedImageExtensions.Contains(ext))
            throw new ImageValidationException(
                IssueCodes.File.UnsupportedFormat,
                "Поддерживаются только JPG и PNG.",
                imageName);
        
        var image = Image.Create(ext);

        using MemoryStream memoryStream = new(content);

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = image.Path,
            InputStream = memoryStream,
            ContentType = mime,
        };

        await _s3Client.PutObjectAsync(request, ct);
        await _imageRepository.AddAsync(image, ct);

        return image;
    }
    
    public async Task UpdateEmbeddingAsync(Guid imageId, Vector vector, AnnouncementType announcementType, CancellationToken ct)
    {
        await using var transaction = await _transactionFactory.BeginAsync(ct);
        
        var image = await _imageRepository.GetByIdAsync(imageId, ct);
        image.SetEmbedding(vector);
        
        var outboxPayload = JsonSerializer.Serialize(new EmbedRequest{
            ImageId = image.Id,
        });

        if (announcementType == AnnouncementType.Street)
        {
            var outboxEvent = OutboxEvent.Create("image.find.similar.missing", outboxPayload);
            await _outboxRepository.AddAsync(outboxEvent, ct);
        }
        await _imageRepository.UpdateAsync(image, ct);

        await transaction.CommitAsync(ct);
    }

    public async Task<string> PutInS3(byte[] content, CancellationToken ct, string? imageName = null)
    {
        var (ext, mime) = DetectImage(content);
        if (!AllowedImageExtensions.Contains(ext))
            throw new ImageValidationException(
                IssueCodes.File.UnsupportedFormat,
                "Поддерживаются только JPG и PNG.",
                imageName);

        using MemoryStream memoryStream = new(content);

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = Guid.NewGuid() + ext,
            InputStream = memoryStream,
            ContentType = mime,
        };

        await _s3Client.PutObjectAsync(request, ct);

        return request.Key;
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
    
    public async Task<ImageResponseInfo> DownloadFileAsync(string key, CancellationToken ct)
    {
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = key
        };
        
        var extension = Path.GetExtension(key);

        var response = await _s3Client.GetObjectAsync(request, ct);
        
        return new ImageResponseInfo
        {
            ResponseStream = response.ResponseStream,
            MimeType = MimeTypes[extension],
        };
    }
    
    public async Task DeleteImageAsync(Guid id, string imagePath, CancellationToken ct)
    {
        var request = new DeleteObjectRequest()
        {
            BucketName = _bucketName,
            Key = imagePath
        };
        
        await _s3Client.DeleteObjectAsync(request, ct);
        await _imageRepository.DeleteAsync(id, ct);
    }
}