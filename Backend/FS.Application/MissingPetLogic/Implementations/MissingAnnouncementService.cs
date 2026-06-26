using FS.Application.AnnouncementLogic.Policies;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Storages;
using FS.Application.Interfaces.Transaction;
using FS.Application.MissingPetLogic.DTOs;
using FS.Application.MissingPetLogic.Interfaces;
using FS.Application.Shared.Configurations;
using FS.Application.Shared.DTOs;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.ImageDomain.Entities;
using FS.Core.Shared.ValueObjects;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace FS.Application.MissingPetLogic.Implementations;

public class MissingAnnouncementService(
    IMissingAnnouncementRepository missingAnnouncementRepository,
    IImageStorageService imageStorageService,
    ITransactionFactory transactionFactory,
    IMissingAnnouncementQueryService missingAnnouncementQueryService,
    IOptions<S3StorageConfiguration> s3StorageOptions,
    ISpottedLocationsQueryService spottedLocationsQueryService,
    IFoundReportsQueryService foundReportsQueryService,
    GeometryFactory geometryFactory) 
    : IMissingAnnouncementService
{
    private readonly S3StorageConfiguration _s3StorageConfiguration = s3StorageOptions.Value;
    
    public async Task<MissingAnnouncementFeed[]> GetFeedAsync(
        AnnouncementFilter announcementFilter,
        DateTime? lastDateTime = null,
        CancellationToken ct = default)
    {
        Point? centerSearchPoint = null;
        if (announcementFilter.SearchCenter != null)
        {
            centerSearchPoint = geometryFactory.CreatePoint(new Coordinate(
                announcementFilter.SearchCenter.Longitude,
                announcementFilter.SearchCenter.Latitude));
        }
        
        var missingAnnouncementSpecification = new PetAnnouncementFeedSpecification<MissingAnnouncement>
            (announcementFilter.District,
            announcementFilter.From,
            announcementFilter.Type,
            announcementFilter.Gender,
            centerSearchPoint,
            announcementFilter.SearchRadius,
            null,
            a => a.Images);
        
        var feed = await missingAnnouncementQueryService.GetFeedAsync(
                missingAnnouncementSpecification,
                lastDateTime,
                ct);
        
        return feed;
    }

    public async Task Create(CreateMissingAnnouncementData data, CancellationToken ct)
    {
        await using var transaction = await transactionFactory.BeginAsync(ct);
        
        List<FSImage> images = [];
        
        foreach (var image in data.Images)
        {
            var s3Key = Guid.NewGuid().ToString();
            var createdImage = FSImage.Create(s3Key, _s3StorageConfiguration.ImagesBucketUrl);
            images.Add(createdImage);
            await imageStorageService.UploadAsync(image.Content, s3Key, ct);
        }
        
        var coordinates = geometryFactory.CreatePoint(new Coordinate(
            data.Location.Longitude,
            data.Location.Latitude));
        
        var missingAnnouncement = MissingAnnouncement.Create(
            street: data.Street,
            house: data.House,
            images,
            data.CreatorId,
            data.District,
            data.PetType,
            data.Gender,
            data.Color,
            data.Breed,
            coordinates,
            data.PetName,
            data.EventDate,
            data.Description
        );

        await missingAnnouncementRepository.CreateAsync(missingAnnouncement, ct);
        
        await transaction.CommitAsync(ct);
    }

    public async Task<MissingAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct) =>
        await missingAnnouncementQueryService.GetForPageByIdAsync(id, ct);

    public async Task Cancel(DeleteMissingAnnouncementData data, CancellationToken ct)
    {
        var announcement = await missingAnnouncementRepository.GetByIdAsync(data.AnnouncementId, ct);

        var deletionPolicy = new DefaultAnimalAnnouncementDeletionPolicy()
        {
            UserId = data.DeleterId,
            Announcement = announcement
        };
        
        announcement.Cancel(data.DeleteReason, deletionPolicy);
        
        await missingAnnouncementRepository.SaveChangesAsync(ct);
    }

    public async Task<MyAnnouncementFeed[]> GetFeedItemsByCreatorByPage(
        Guid creatorId,
        DateTime? lastDateTime = null,
        CancellationToken ct = default) =>
    await missingAnnouncementQueryService.GetFeedForUserAsync(creatorId, lastDateTime, ct);

    public async Task ReportSpottedAsync(SpottedInfo spottedInfo, CancellationToken ct)
    {
        var coordinatesVO = CoordinatesVO.Create(spottedInfo.Location.Latitude, spottedInfo.Location.Longitude);
        var announcement = await missingAnnouncementRepository.GetByIdAsync(spottedInfo.AnnouncementId, ct);
        
        List<FSImage> images = [];
        
        foreach (var image in spottedInfo.Images)
        {
            var s3Key = Guid.NewGuid().ToString();
            var createdImage = FSImage.Create(s3Key, _s3StorageConfiguration.ImagesBucketUrl);
            images.Add(createdImage);
            await imageStorageService.UploadAsync(image.Content, s3Key, ct);
        }
        
        announcement.ReportSpotted(spottedInfo.SpottedUserId, coordinatesVO, images);
        
        await missingAnnouncementRepository.SaveChangesAsync(ct);
    }
    
    public async Task ReportFoundAsync(FoundInfo foundInfo, CancellationToken ct)
    {
        var announcement = await missingAnnouncementRepository.GetByIdAsync(foundInfo.AnnouncementId, ct);
        
        List<FSImage> images = [];
        
        foreach (var image in foundInfo.Images)
        {
            var s3Key = Guid.NewGuid().ToString();
            var createdImage = FSImage.Create(s3Key, _s3StorageConfiguration.ImagesBucketUrl);
            images.Add(createdImage);
            await imageStorageService.UploadAsync(image.Content, s3Key, ct);
        }
        
        announcement.ReportFound(foundInfo.FoundUserId, images);
        await missingAnnouncementRepository.SaveChangesAsync(ct);
    }

    public async Task<SpottedLocationDto[]> GetSpottedLocations(Guid missingAnnouncementId, CancellationToken ct) =>
        await spottedLocationsQueryService.GetSpottedLocationsByAnnouncementIdAsync(missingAnnouncementId, ct);
    
    public async Task<FoundReportDto[]> GetFoundReports(Guid missingAnnouncementId, CancellationToken ct) =>
        await foundReportsQueryService.GetFoundReportsByAnnouncementIdAsync(missingAnnouncementId, ct);
}