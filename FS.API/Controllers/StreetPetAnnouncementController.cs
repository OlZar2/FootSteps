using System.Security.Claims;
using FluentValidation;
using FS.API.Errors;
using FS.API.RequestsModels.StreetPetAnnouncement;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.API.Services.ImageLogic;
using FS.Application.DTOs.Shared;
using FS.Application.DTOs.StreetPetAnnouncementDTOs;
using FS.Application.Services.StreetPetAnnouncementLogic.Interfaces;
using FS.Contracts.Error;
using FS.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

[ApiController]
[Route("api/street-pet-announcement")]
public class StreetPetAnnouncementController(
    IStreetPetAnnouncementService streetPetAnnouncementService,
    IValidator<CreateStreetPetAnnouncementRM> createStreetPetAnnouncementValidator,
    IClaimService claimService,
    ImageService imageService) : ControllerBase
{
    /// <summary>
    /// Создание объявлений о замеченых питомцах
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreatedStreetPetAnnouncement), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)] 
    [Authorize]
    public async Task<CreatedStreetPetAnnouncement> Create([FromForm] CreateStreetPetAnnouncementRM request, CancellationToken ct)
    {
        await createStreetPetAnnouncementValidator.ValidateAndThrowAsync(request, ct);
        
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);

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

    /// <summary>
    /// Возвращает список из 20 объявлений о замеченых животных.
    /// </summary>
    /// <param name="lastDateTime">Дата и время последнего полученного обяъвления о пропаже(для пагинации)</param>
    /// <param name="filter">Фильтр объявлений (например, по типу, категории и т. д.).</param>
    /// <param name="ct"></param>
    [HttpGet("feed")]
    [ProducesResponseType(typeof(StreetPetAnnouncementFeed[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<StreetPetAnnouncementFeed[]> GetFeed(
        [FromQuery] DateTime lastDateTime,
        [FromQuery] StreetPetAnnouncementFilter filter,
        CancellationToken ct)
    {
        var response = await streetPetAnnouncementService.GetFeedAsync(lastDateTime, filter, ct);
        return response;
    }

    /// <summary>
    /// Подробная информация об объявлении
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StreetPetAnnouncementPage), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<StreetPetAnnouncementPage> GetPageByIdAsync(Guid id, CancellationToken ct)
    {
        return await streetPetAnnouncementService.GetPageByIdAsync(id, ct);
    }
}