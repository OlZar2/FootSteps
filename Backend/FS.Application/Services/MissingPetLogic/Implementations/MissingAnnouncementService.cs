using FS.Application.DomainPolicies.AnimalAnnouncementPolicies;
using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Storages;
using FS.Application.Interfaces.Transaction;
using FS.Application.Services.ImageLogic.Configurations;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Application.Services.MissingPetLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Specifications;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.ImageDomain.Entities;
using FS.Core.Shared.ValueObjects;
using Microsoft.Extensions.Options;

namespace FS.Application.Services.MissingPetLogic.Implementations;

public class MissingAnnouncementService(
    IMissingAnnouncementRepository missingAnnouncementRepository,
    IImageRepository imageRepository,
    ITransactionFactory transactionFactory,
    IMissingAnnouncementQueryService missingAnnouncementQueryService,
    IOptions<S3StorageConfiguration> s3StorageOptions) 
    : IMissingAnnouncementService
{
    private readonly S3StorageConfiguration _s3StorageConfiguration = s3StorageOptions.Value;
    
    public async Task<MissingAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime,
        AnnouncementFilter announcementFilter, CancellationToken ct)
    {
        var missingAnnouncementSpecification = new PetAnnouncementFeedSpecification<MissingAnnouncement>
            (announcementFilter.District,
            announcementFilter.From,
            announcementFilter.Type,
            announcementFilter.Gender,
            null,
            a => a.Images);
        
        var feed = await missingAnnouncementQueryService.
            GetFilteredByPageAsync(lastDateTime, missingAnnouncementSpecification, ct);
        
        return feed;
    }

    public async Task Create(CreateMissingAnnouncementData data, CancellationToken ct)
    {
        await using var transaction = await transactionFactory.BeginAsync(ct);
        
        var images = await imageRepository.GetByIdsAsync(data.ImageIds, ct);
        
        var coordinates = CoordinatesVO.Create(data.Location.Latitude, data.Location.Longitude);
        
        var missingAnnouncement = MissingAnnouncement.Create(
            street: data.Street,
            house: data.House,
            images.ToList(),
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
        
        await missingAnnouncementRepository.UpdateAsync(announcement, ct);
    }

    public async Task<MyAnnouncementFeed[]> GetFeedItemsByCreatorByPage(
        Guid creatorId,
        DateTime lastDateTime,
        CancellationToken ct) =>
    await missingAnnouncementQueryService.GetFeedForUserAsync(creatorId, lastDateTime, ct);
}