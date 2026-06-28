using FS.Application.AnnouncementLogic.Policies;
using FS.Application.FindAnnouncementLogic.DTOs;
using FS.Application.FindAnnouncementLogic.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Storages;
using FS.Application.Interfaces.Transaction;
using FS.Application.Shared.Configurations;
using FS.Application.Shared.DTOs;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.ImageDomain.Entities;
using FS.Core.Shared.ValueObjects;
using Microsoft.Extensions.Options;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Application.FindAnnouncementLogic.Implementations;

public class FindAnnouncementService(
    IFindAnnouncementRepository findAnnouncementRepository,
    IFindAnnouncementQueryService findAnnouncementQueryService,
    IAnimalAnnouncementQueryService animalAnnouncementQueryService,
    ITransactionFactory transactionFactory,
    IImageStorageService imageStorageService,
    IOptions<S3StorageConfiguration> s3StorageOptions,
    GeometryFactory geometryFactory) : IFindAnnouncementService
{
    private readonly S3StorageConfiguration _s3StorageConfiguration = s3StorageOptions.Value;
    
    public async Task Create(CreateFindAnnouncementData data, CancellationToken ct)
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

        var findAnnouncement = FindAnnouncement.Create(
            street:data.Street,
            house:data.House,
            images,
            data.CreatorId,
            data.District,
            data.PetType,
            data.Gender,
            data.Color,
            data.Breed,
            coordinates,
            data.EventDate,
            data.Description);
        
        await findAnnouncementRepository.CreateAsync(findAnnouncement, ct);
        
        await transaction.CommitAsync(ct);
    }

    public async Task<FindAnnouncementFeed[]> GetFeedAsync(
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
        
        var findAnnouncementSpecification = new PetAnnouncementFeedSpecification<FindAnnouncement>
        (announcementFilter.District,
            announcementFilter.From,
            announcementFilter.Type,
            announcementFilter.Gender,
            centerSearchPoint,
            announcementFilter.SearchRadius,
            null,
            a => a.Images);
        
        var feed = await findAnnouncementQueryService.GetFeedAsync(
            findAnnouncementSpecification,
            lastDateTime,
            centerSearchPoint,
            announcementFilter.SearchRadius,
            ct);
        
        return feed;
    }

    public async Task<FindAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        var deleteType = await animalAnnouncementQueryService.GetDeleteTypeByIdAsync(id, ct);
        AnimalAnnouncementVisibilityPolicy.EnsureVisibleForPage(deleteType);

        return await findAnnouncementQueryService.GetForPageByIdAsync(id, ct);
    }
    
    public async Task Cancel(DeleteFindAnnouncementData data, CancellationToken ct)
    {
        var announcement = await findAnnouncementRepository.GetByIdAsync(data.AnnouncementId, ct);

        var deletionPolicy = new DefaultAnimalAnnouncementDeletionPolicy()
        {
            UserId = data.DeleterId,
            Announcement = announcement
        };
        
        announcement.Cancel(data.DeleteReason, deletionPolicy);
        
        await findAnnouncementRepository.UpdateAsync(announcement, ct);
    }

    public async Task<MyAnnouncementFeed[]> GetFeedItemsByCreatorByPage(
        Guid userId,
        DateTime? lastDateTime = null,
        CancellationToken ct = default)
    => await findAnnouncementQueryService.GetFeedForUserAsync(userId, lastDateTime, ct);
}
