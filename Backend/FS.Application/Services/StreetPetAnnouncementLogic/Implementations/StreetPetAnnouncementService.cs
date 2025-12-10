using FS.Application.DTOs.Shared;
using FS.Application.DTOs.StreetPetAnnouncementDTOs;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Transaction;
using FS.Application.Services.ImageLogic.Configurations;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Application.Services.StreetPetAnnouncementLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Entities;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.ReadDomain;
using FS.Core.ReadDomain.Stores;
using FS.Core.Shared.ValueObjects;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FS.Application.Services.StreetPetAnnouncementLogic.Implementations;

public class StreetPetAnnouncementService(
    IStreetPetAnnouncementRepository streetPetAnnouncementRepository,
    ITransactionFactory transactionFactory,
    IImageStorageService imageStorageService,
    IStreetPetAnnouncementQueryService streetPetAnnouncementQueryService,
    IMissingAnnouncementRepository missingAnnouncementRepository,
    ILogger<StreetPetAnnouncementService> logger,
    ISimilarAnnouncementRepository similarAnnouncementRepository,
    IOptions<S3StorageConfiguration> s3Options)
    : IStreetPetAnnouncementService
{
    private readonly S3StorageConfiguration _s3StorageConfiguration = s3Options.Value;
    
    public async Task CreateAsync(CreateStreetPetAnnouncementData data, CancellationToken ct)
    {
        await using var transaction = await transactionFactory.BeginAsync(ct);
        
        var coordinates = CoordinatesVO.Create(data.Location.Latitude, data.Location.Latitude);
        
        var images = new List<AnimalAnnouncementImage>();
        foreach (var image in data.Images)
        {
            var s3Key = Guid.NewGuid().ToString();
            var createdImage = AnimalAnnouncementImage.Create(s3Key, _s3StorageConfiguration.ImagesBucketUrl);
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

    public async Task<StreetPetAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime, StreetPetAnnouncementFilter filter,
        CancellationToken ct)
    {
        //TODO: подумать нужна ли спецификация
        var specification = new StreetPetAnnouncementFeedSpecification(
            filter.District,
            filter.From,
            filter.Type,
            null,
            a => a.Images);
        
        var response = 
            await streetPetAnnouncementQueryService.GetFeedAsync(lastDateTime, specification, ct);
        return response;
    }

    public async Task<StreetPetAnnouncementPage> GetPageByIdAsync(Guid id, CancellationToken ct)
    {
        var response = await streetPetAnnouncementQueryService.GetForPageByIdAsync(id, ct);
        
        return response;
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