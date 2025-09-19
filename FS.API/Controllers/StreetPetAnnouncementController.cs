using System.Security.Claims;
using FluentValidation;
using FS.API.RequestsModels.StreetPetAnnouncement;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application.DTOs.Shared;
using FS.Application.DTOs.StreetPetAnnouncementDTOs;
using FS.Application.Services.StreetPetAnnouncementLogic.Interfaces;
using FS.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

[ApiController]
[Route("api/street-pet-announcement")]
public class StreetPetAnnouncementController(
    IStreetPetAnnouncementService streetPetAnnouncementService,
    IValidator<CreateStreetPetAnnouncementRM> createStreetPetAnnouncementValidator,
    IClaimService claimService) : ControllerBase
{
    /// <summary>
    /// Создание объявлений о замеченых питомцах
    /// </summary>
    /// <param name="request"></param>
    /// <param name="ct"></param>
    [HttpPost]
    [Authorize]
    public async Task<CreatedStreetPetAnnouncement> Create([FromForm] CreateStreetPetAnnouncementRM request, CancellationToken ct)
    {
        await createStreetPetAnnouncementValidator.ValidateAndThrowAsync(request, ct);
        
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);
        
        //TODO: в севрис
        var semaphore = new SemaphoreSlim(4);

        var tasks = request.Images.Select(async image =>
        {
            await semaphore.WaitAsync(ct);
            try
            {
                await using var ms = new MemoryStream();
                await image.CopyToAsync(ms, ct);

                return new FileData
                {
                    Content = ms.ToArray()
                };
            }
            finally
            {
                semaphore.Release();
            }
        });

        var fileInfos = await Task.WhenAll(tasks);

        var dto = new CreateStreetPetAnnouncementData
        {
            CreatorId = userId,
            District = request.District,
            EventDate = request.EventDate!.Value,
            Location = request.Location,
            FullPlace = request.FullPlace,
            PetType = (PetType)request.PetType!.Value,
            Images = fileInfos,
            PlaceDescription = request.PlaceDescription,
        };

        var response = await streetPetAnnouncementService.CreateAsync(dto, ct);
        return response;
    }

    [HttpGet("feed")]
    public async Task<StreetPetAnnouncementFeed[]> GetFeed(
        [FromQuery] DateTime lastDateTime,
        [FromQuery] StreetPetAnnouncementFilter filter,
        CancellationToken ct)
    {
        var response = await streetPetAnnouncementService.GetFeedAsync(lastDateTime, filter, ct);
        return response;
    }
}