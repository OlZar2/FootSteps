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
using FS.Core.ValueObjects;
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
        
        var feed = await missingAnnouncementRepository.
            GetFilteredByPageAsync(lastDateTime, missingAnnouncementSpecification, ct);

        var response =  feed.Select(a => new MissingAnnouncementFeed
        {
            Id = a.Id,
            PetName = a.PetName,
            CreatedAt = a.CreatedAt,
            District = a.District.Value,
            PetType = a.PetType,
            Gender = a.Gender,
            MainImagePath = a.Images[0].Path,
        }).ToArray();
        
        return response;
    }

    public async Task<CreatedMissingAnnouncement> Create(CreateMissingAnnouncementData data, CancellationToken ct)
    {
        return await transactionService.ExecuteInTransactionAsync(async () =>
        {
            var place = Place.Create(data.FullPlace);
            var district = District.Create(data.District);
            
            //TODO: может можно вынести в DI
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var point = geometryFactory.CreatePoint(new Coordinate(data.Location.Longitude, data.Location.Latitude));
            
            //TODO: сделать параллельно
            var images = new List<Image>();
            foreach (var image in  data.Images)
            {
                var createdImage = await imageService.CreateImageAsync(image.Content, ct, nameof(data.Images));
                images.Add(createdImage);
            }
            
            var missingAnnouncement = MissingAnnouncement.Create(
                place,
                images,
                data.CreatorId,
                district,
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

            var response = await missingAnnouncementQueryService
                .GetCreatedByIdAsync(missingAnnouncement.Id, ct);
            
            return response;
        }, ct);
    }

    public async Task<MissingAnnouncementPage> GetForPageByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await missingAnnouncementQueryService.GetForPageByIdAsync(id, ct);
        
        return entity;
    }

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
}