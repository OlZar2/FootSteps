using FS.Application.DTOs.Shared;
using FS.Application.DTOs.StreetPetAnnouncementDTOs;
using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Transaction;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Application.Services.StreetPetAnnouncementLogic.Interfaces;
using FS.Core.Entities;
using FS.Core.Enums;
using FS.Core.Specifications;
using FS.Core.Stores;
using Microsoft.Extensions.Logging;
using NetTopologySuite;
using NetTopologySuite.Geometries;

namespace FS.Application.Services.StreetPetAnnouncementLogic.Implementations;

public class StreetPetAnnouncementService(
    IStreetPetAnnouncementRepository streetPetAnnouncementRepository,
    ITransactionFactory transactionFactory,
    IImageService imageService,
    IStreetPetAnnouncementQueryService streetPetAnnouncementQueryService,
    IMissingAnnouncementRepository missingAnnouncementRepository,
    IImageRepository imageRepository,
    ILogger<StreetPetAnnouncementService> logger)
    : IStreetPetAnnouncementService
{
    public async Task CreateAsync(CreateStreetPetAnnouncementData data, CancellationToken ct)
    {
        await using var transaction = await transactionFactory.BeginAsync(ct);
        
        //TODO: может можно вынести в DI
        var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
        var point = geometryFactory.CreatePoint(new Coordinate(data.Location.Longitude, data.Location.Latitude));

        //TODO: сделать параллельно
        var images = new List<Image>();
        foreach (var image in data.Images)
        {
            var createdImage = await imageService.CreateImageForAnnouncementAsync(
                image.Content,
                AnnouncementType.Street,
                ct,
                nameof(data.Images));
            images.Add(createdImage);
        }

        var streetPetAnnouncement = StreetPetAnnouncement.Create(
            street:data.Street,
            house:data.House,
            images,
            data.CreatorId,
            data.District,
            data.PetType,
            point,
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

        var image = imageRepository.GetByIdAsync(streetAnnouncementImageId, ct).Result;
        if (image.Embedding == null)
        {
            logger.LogWarning("Embedding is null when finding similar announcements");
            return;
        }
        
        var similarMissingAnnouncements = await missingAnnouncementRepository
            .GetSimilarMissingAnnouncementAsync(image.Embedding, ct);
        streetPetAnnouncement.AddSimilarMissingAnnouncements(similarMissingAnnouncements);
    }
}