using FS.Application.Interfaces.QueryServices;
using FS.Application.AnnouncementLogic.Policies;
using FS.Application.Interfaces.Storages;
using FS.Application.Interfaces.Transaction;
using FS.Application.Shared.Configurations;
using FS.Application.StreetPetAnnouncementLogic.DTOs;
using FS.Application.StreetPetAnnouncementLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.ImageDomain.Entities;
using FS.Core.ReadDomain;
using FS.Core.ReadDomain.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace FS.Application.StreetPetAnnouncementLogic.Implementations;

public class StreetPetAnnouncementService(
    IStreetPetAnnouncementRepository streetPetAnnouncementRepository,
    ITransactionFactory transactionFactory,
    IImageStorageService imageStorageService,
    IStreetPetAnnouncementQueryService streetPetAnnouncementQueryService,
    IAnimalAnnouncementQueryService animalAnnouncementQueryService,
    IMissingAnnouncementRepository missingAnnouncementRepository,
    ILogger<StreetPetAnnouncementService> logger,
    ISimilarAnnouncementRepository similarAnnouncementRepository,
    IOptions<S3StorageConfiguration> s3Options,
    GeometryFactory geometryFactory)
    : IStreetPetAnnouncementService
{
    private readonly S3StorageConfiguration _s3StorageConfiguration = s3Options.Value;
    
    public async Task CreateAsync(CreateStreetPetAnnouncementData data, CancellationToken ct)
    {
        await using var transaction = await transactionFactory.BeginAsync(ct);
        
        var coordinates = geometryFactory.CreatePoint(new Coordinate(
            data.Location.Longitude,
            data.Location.Latitude));
        
        List<FSImage> images = [];
        
        foreach (var image in data.Images)
        {
            var s3Key = Guid.NewGuid().ToString();
            var createdImage = FSImage.Create(s3Key, _s3StorageConfiguration.ImagesBucketUrl);
            images.Add(createdImage);
            await imageStorageService.UploadAsync(image.Content, s3Key, ct);
        }

        var streetPetAnnouncement = StreetPetAnnouncement.Create(
            street:data.Street,
            house:data.House,
            images,
            data.CreatorId,
            data.District,
            data.PetType,
            coordinates,
            data.EventDate,
            data.PlaceDescription);

        await streetPetAnnouncementRepository.CreateAsync(streetPetAnnouncement, ct);

        await transaction.CommitAsync(ct);
    }

    public async Task<StreetPetAnnouncementFeed[]> GetFeedAsync(
        StreetPetAnnouncementFilter filter,
        DateTime? lastDateTime = null,
        CancellationToken ct = default)
    {
        Point? centerSearchPoint = null;
        if (filter.SearchCenter != null)
        {
            centerSearchPoint = geometryFactory.CreatePoint(new Coordinate(
                filter.SearchCenter.Longitude,
                filter.SearchCenter.Latitude));
        }
        
        var specification = new StreetPetAnnouncementFeedSpecification(
            filter.District,
            filter.From,
            filter.Type,
            centerSearchPoint,
            filter.SearchRadius,
            null,
            a => a.Images);
        
        var response = await streetPetAnnouncementQueryService.GetFeedAsync(
                specification,
                lastDateTime,
                ct);
        
        return response;
    }

    public async Task<StreetPetAnnouncementPage> GetPageByIdAsync(Guid id, CancellationToken ct)
    {
        var deleteType = await animalAnnouncementQueryService.GetDeleteTypeByIdAsync(id, ct);
        AnimalAnnouncementVisibilityPolicy.EnsureVisibleForPage(deleteType);

        return await streetPetAnnouncementQueryService.GetForPageByIdAsync(id, ct);
    }

    public async Task UpdateSimilarAnnouncementAsync(
        Guid streetAnnouncementImageId,
        CancellationToken ct)
    {
        var streetPetAnnouncement = await streetPetAnnouncementRepository.GetByImageIdAsync(
            streetAnnouncementImageId,
            ct);
        if (streetPetAnnouncement is null)
        {
            logger.LogWarning("Announcement is null when finding similar announcements");
            return;
        }
        
        var image = streetPetAnnouncement.Images.FirstOrDefault(i => i.Id == streetAnnouncementImageId);
        if (image?.Embedding == null)
        {
            logger.LogWarning("Embedding is null when finding similar announcements");
            return;
        }
        
        var similarMissingAnnouncements = await missingAnnouncementRepository
            .GetSimilarMissingAnnouncementAsync(image.Embedding, ct);
        
        //TODO: Возможно это тожн лучше через domainEvent
        var similarReadModels = similarMissingAnnouncements.Select(sma => new SimilarAnnouncements
        {
            StreetPetAnnouncementId = streetAnnouncementImageId,
            MissingAnnouncementId = sma.Id,
        });
        await similarAnnouncementRepository.AddRangeAsync(similarReadModels, ct);
    }
}
