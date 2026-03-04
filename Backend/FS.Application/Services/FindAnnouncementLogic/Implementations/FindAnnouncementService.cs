using FS.Application.Configurations;
using FS.Application.DomainPolicies.AnimalAnnouncementPolicies;
using FS.Application.DTOs.FindAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Storages;
using FS.Application.Interfaces.Transaction;
using FS.Application.Services.FindAnnouncementLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.ImageDomain.Entities;
using FS.Core.Shared.ValueObjects;
using Microsoft.Extensions.Options;

namespace FS.Application.Services.FindAnnouncementLogic.Implementations;

public class FindAnnouncementService(
    IFindAnnouncementRepository findAnnouncementRepository,
    IFindAnnouncementQueryService findAnnouncementQueryService,
    ITransactionFactory transactionFactory,
    IImageStorageService imageStorageService,
    IImageRepository imageRepository,
    IOptions<S3StorageConfiguration> s3StorageOptions) : IFindAnnouncementService
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

        var coordinates = CoordinatesVO.Create(data.Location.Latitude, data.Location.Longitude);

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

    public async Task<FindAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime,
        AnnouncementFilter announcementFilter, CancellationToken ct)
    {
        var findAnnouncementSpecification = new PetAnnouncementFeedSpecification<FindAnnouncement>
        (announcementFilter.District,
            announcementFilter.From,
            announcementFilter.Type,
            announcementFilter.Gender,
            null,
            a => a.Images);
        
        var feed = await findAnnouncementQueryService.GetFeedAsync(
            lastDateTime,
            findAnnouncementSpecification,
            ct);
        
        return feed;
    }

    public async Task<FindAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await findAnnouncementQueryService.GetForPageByIdAsync(id, ct);
        
        return entity;
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
        DateTime lastDateTime,
        CancellationToken ct)
    => await findAnnouncementQueryService.GetFeedForUserAsync(userId, lastDateTime, ct);
}