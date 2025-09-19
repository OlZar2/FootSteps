using FS.Application.DTOs.Shared;
using FS.Application.DTOs.StreetPetAnnouncementDTOs;
using FS.Application.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Application.Services.StreetPetAnnouncementLogic.Interfaces;
using FS.Core.Entities;
using FS.Core.Specifications;
using FS.Core.Stores;
using FS.Core.ValueObjects;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Application.Services.StreetPetAnnouncementLogic.Implementations;

public class StreetPetAnnouncementService(
    IStreetPetAnnouncementRepository streetPetAnnouncementRepository,
    ITransactionService transactionService,
    IImageService imageService,
    IStreetPetAnnouncementQueryService streetPetAnnouncementQueryService)
    : IStreetPetAnnouncementService
{
    public async Task<CreatedStreetPetAnnouncement> CreateAsync(CreateStreetPetAnnouncementData data, CancellationToken ct)
    {
        return await transactionService.ExecuteInTransactionAsync(async () =>
        {
            //TODO: может можно вынести в DI
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var point = geometryFactory.CreatePoint(new Coordinate(data.Location.Longitude, data.Location.Latitude));

            //TODO: сделать параллельно
            var images = new List<Image>();
            foreach (var image in data.Images)
            {
                var createdImage = await imageService.CreateImageAsync(image.Content, ct, nameof(data.Images));
                images.Add(createdImage);
            }

            var fullPlace = Place.Create(data.FullPlace);
            var district = District.Create(data.District);

            var streetPetAnnouncement = StreetPetAnnouncement.Create(
                fullPlace,
                images,
                data.CreatorId,
                district,
                data.PetType,
                point,
                data.EventDate,
                data.PlaceDescription);

            await streetPetAnnouncementRepository.CreateAsync(streetPetAnnouncement, ct);

            var response = 
                await streetPetAnnouncementQueryService.GetCreatedByIdAsync(streetPetAnnouncement.Id, ct);
            
            return response;
        }, ct);
    }

    public async Task<StreetPetAnnouncementFeed[]> GetFeedAsync(DateTime lastDateTime, StreetPetAnnouncementFilter filter,
        CancellationToken ct)
    {
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
}