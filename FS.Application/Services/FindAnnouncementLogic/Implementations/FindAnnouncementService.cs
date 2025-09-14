using FS.Application.DomainPolicies.AnimalAnnouncementPolicies;
using FS.Application.DTOs.FindAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Services.FindAnnouncementLogic.Interfaces;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Core.Entities;
using FS.Core.Specifications;
using FS.Core.Stores;
using FS.Core.ValueObjects;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Application.Services.FindAnnouncementLogic.Implementations;

public class FindAnnouncementService(
    IFindAnnouncementRepository findAnnouncementRepository,
    IFindAnnouncementQueryService findAnnouncementQueryService,
    ITransactionService transactionService,
    IImageService imageService) : IFindAnnouncementService
{
    public async Task<CreatedFindAnnouncement> Create(CreateFindAnnouncementData data, CancellationToken ct)
    {
        return await transactionService.ExecuteInTransactionAsync(async () =>
        {
            var place = Place.Create(data.FullPlace);
            var district = District.Create(data.District);

            var images = new List<Image>();
            foreach (var image in data.Images)
            {
                var createdImage = await imageService.CreateImageAsync(image.Content, ct, nameof(data.Images));
                images.Add(createdImage);
            }

            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var point = geometryFactory.CreatePoint(new Coordinate(data.Location.Longitude, data.Location.Latitude));

            var findAnnouncement = FindAnnouncement.Create(
                place,
                images,
                data.CreatorId,
                district,
                data.PetType,
                data.Gender,
                data.Color,
                data.Breed,
                point,
                data.EventDate,
                data.Description);
            
            await findAnnouncementRepository.CreateAsync(findAnnouncement, ct);

            var response = await 
                findAnnouncementQueryService.GetCreatedFindAnnouncement(findAnnouncement.Id, ct);
            return response;
        }, ct);
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
    
    public async Task Delete(DeleteFindAnnouncementData data, CancellationToken ct)
    {
        var announcement = await findAnnouncementRepository.GetByIdAsync(data.AnnouncementId, ct);

        var deletionPolicy = new DefaultAnimalAnnouncementDeletionPolicy()
        {
            UserId = data.DeleterId,
            Announcement = announcement
        };
        
        announcement.Delete(data.DeleteReason, deletionPolicy);
        
        await findAnnouncementRepository.UpdateAsync(announcement, ct);
    }
}