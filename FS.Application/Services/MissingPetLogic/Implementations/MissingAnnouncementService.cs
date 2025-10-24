using FS.Application.DomainPolicies.AnimalAnnouncementPolicies;
using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Application.Services.MissingPetLogic.Interfaces;
using FS.Core.Entities;
using FS.Core.Specifications;
using FS.Core.Stores;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Application.Services.MissingPetLogic.Implementations;

public class MissingAnnouncementService(
    IMissingAnnouncementRepository missingAnnouncementRepository,
    IImageService imageService,
    ITransactionService transactionService,
    IMissingAnnouncementQueryService missingAnnouncementQueryService) 
    : IMissingAnnouncementService
{
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
        
        //TODO: переенести из репозитория в queryService
        var feed = await missingAnnouncementRepository.
            GetFilteredByPageAsync(lastDateTime, missingAnnouncementSpecification, ct);

        var response =  feed.Select(a => new MissingAnnouncementFeed
        {
            Id = a.Id,
            PetName = a.PetName,
            CreatedAt = a.CreatedAt,
            District = a.District,
            PetType = a.PetType,
            Gender = a.Gender,
            MainImagePath = a.Images[0].Path,
            Description = a.Description,
        }).ToArray();
        
        return response;
    }

    public async Task Create(CreateMissingAnnouncementData data, CancellationToken ct)
    {
        await transactionService.ExecuteInTransactionAsync(async () =>
        {
            //TODO: может можно вынести в DI
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var point = geometryFactory.CreatePoint(new Coordinate(data.Location.Longitude, data.Location.Latitude));
            
            //TODO: сделать параллельно
            var images = new List<Image>();
            foreach (var image in  data.Images)
            {
                var createdImage = await imageService.CreateImageForAnnouncementAsync(image.Content, ct, nameof(data.Images));
                images.Add(createdImage);
            }
            
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
                point,
                data.PetName,
                data.EventDate,
                data.Description
            );

            await missingAnnouncementRepository.CreateAsync(missingAnnouncement, ct);
        }, ct);
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