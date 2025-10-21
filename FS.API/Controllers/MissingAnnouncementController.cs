using System.Security.Claims;
using FluentValidation;
using FS.API.Errors;
using FS.API.RequestsModels.MissingAnnouncements;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.API.Services.GeoLogic.Interfaces;
using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Services.MissingPetLogic.Interfaces;
using FS.Contracts.Error;
using FS.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

/// <summary>
/// Методы для работы с объявлениями о пропаже
/// </summary>
[ApiController]
[Route("api/missing-announcement")]
public class MissingAnnouncementController(
    IMissingAnnouncementService missingAnnouncementService,
    IClaimService claimService,
    IValidator<CreateMissingAnnouncementRM> createAnnouncementValidator,
    IValidator<CancelMissingAnnouncementRM> deleteAnnouncementValidator,
    IGeocoder geocoder) : ControllerBase
{
    /// <summary>
    /// Возвращает список из 20 объявлений о пропаже.
    /// </summary>
    /// <param name="lastDateTime">Дата и время последнего полученного обяъвления о пропаже(для пагинации)</param>
    /// <param name="filter">Фильтр объявлений (например, по типу, категории и т. д.).</param>
    [HttpGet("feed")]
    [ProducesResponseType(typeof(MissingAnnouncementFeed[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)] 
    public async Task<MissingAnnouncementFeed[]> GetMissingAnnouncement(
        [FromQuery] DateTime lastDateTime,
        [FromQuery] AnnouncementFilter filter,
        CancellationToken ct)
    {
        var feed = await missingAnnouncementService
            .GetFeedAsync(lastDateTime, filter, ct);

        return feed;
    }
    
    /// <summary>
    /// Создание объявления о пропаже
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task Create(
        [FromForm] CreateMissingAnnouncementRM data,
        CancellationToken ct)
    {
        await createAnnouncementValidator.ValidateAndThrowAsync(data, ct);
        
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);
        
        //TODO: в севрис
        var semaphore = new SemaphoreSlim(4);

        var tasks = data.Images.Select(async image =>
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

        var house = await geocoder.GetHouseOrNull(data.Location, ct);
        var street = await geocoder.GetStreetOrNull(data.Location, ct);
        var district = await geocoder.GetDistrictOrNull(data.Location, ct)
                       ?? await geocoder.GetLocalityOrNull(data.Location, ct);

        var createDTO = new CreateMissingAnnouncementData
        {
            Street = street,
            House = house,
            District = district,
            Location = data.Location,
            Images = fileInfos,
            CreatorId = userId,
            Breed = data.Breed,
            Color = data.Color,
            Gender = (Gender)data.Gender!.Value,
            PetName = data.PetName,
            PetType = (PetType)data.PetType!.Value,
            EventDate = data.EventDate!.Value,
            Description = data.Description,
        };
        
        await missingAnnouncementService.Create(createDTO, ct);
    }

    /// <summary>
    /// Подробная информация об объявлении
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MissingAnnouncementPage), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<MissingAnnouncementPage> GetForPage(Guid id, CancellationToken ct)
    {
        return await missingAnnouncementService.GetForPageByIdAsync(id, ct);
    }
    
    /// <summary>
    /// Отмена объявления
    /// </summary>
    [Authorize]
    [HttpPost("cancel/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelMissingAnnouncementRM data,
        CancellationToken ct)
    {
        await deleteAnnouncementValidator.ValidateAndThrowAsync(data, ct);
        
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);

        var deleteDto = new DeleteMissingAnnouncementData
        {
            AnnouncementId = id,
            DeleterId = userId,
            DeleteReason = (MissingAnnouncementDeleteReason)data.DeleteReason,
        };
        
        await missingAnnouncementService.Cancel(deleteDto, ct);

        return NoContent();
    }

    [Authorize]
    [HttpGet("me/feed")]
    [ProducesResponseType(typeof(MyAnnouncementFeed[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<MyAnnouncementFeed[]> GetUserFeed([FromQuery] DateTime lastDateTime, CancellationToken ct)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);

        return await missingAnnouncementService.GetFeedItemsByCreatorByPage(userId, lastDateTime, ct);
    }
}