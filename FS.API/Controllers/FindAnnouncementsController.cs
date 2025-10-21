using System.Security.Claims;
using FluentValidation;
using FS.API.Errors;
using FS.API.RequestsModels.FindAnnouncements;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.API.Services.GeoLogic.Interfaces;
using FS.Application.DTOs.FindAnnouncementDTOs;
using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Services.FindAnnouncementLogic.Interfaces;
using FS.Contracts.Error;
using FS.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

/// <summary>
/// Методы для работы с объявлениями о нахождении
/// </summary>
[ApiController]
[Route("api/find-announcement")]
public class FindAnnouncementsController(
    IFindAnnouncementService findAnnouncementService,
    IValidator<CreateFindAnnouncementRM> createAnnouncementValidator,
    IValidator<CancelFindAnnouncementRM> deleteAnnouncementValidator,
    IClaimService claimService,
    IGeocoder geocoder) : ControllerBase
{
    /// <summary>
    /// Возвращает список из 20 объялений о нахождении питомца
    /// </summary>
    /// <param name="lastDateTime">Дата и время последнего полученного обяъвления о пропаже(для пагинации)</param>
    /// <param name="filter"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("feed")]
    [ProducesResponseType(typeof(FindAnnouncementFeed[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)] 
    public async Task<FindAnnouncementFeed[]> GetMissingAnnouncement(
        [FromQuery] DateTime lastDateTime,
        [FromQuery] AnnouncementFilter filter,
        CancellationToken ct)
    {
        var feed = await findAnnouncementService
            .GetFeedAsync(lastDateTime, filter, ct);

        return feed;
    }
    
    /// <summary>
    /// Создать объявление о нахождении
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task Create(
        [FromForm] CreateFindAnnouncementRM data,
        CancellationToken ct)
    {
        await createAnnouncementValidator.ValidateAndThrowAsync(data, ct);
        
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);
        
        //TODO: картинки в сервис
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

        var createDTO = new CreateFindAnnouncementData
        {
            House = house,
            Street = street,
            District = district,
            Location = data.Location,
            Images = fileInfos,
            CreatorId = userId,
            Breed = data.Breed,
            Color = data.Color,
            Gender = (Gender)data.Gender!.Value,
            PetType = (PetType)data.PetType!.Value,
            EventDate = data.EventDate!.Value,
            Description = data.Description,
        };
        
        await findAnnouncementService.Create(createDTO, ct);
    }
    
    /// <summary>
    /// Получить подробную информацию об объявлении
    /// </summary>
    /// <param name="data"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MissingAnnouncementPage), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<FindAnnouncementPage> GetForPage(Guid id, CancellationToken ct)
    {
        return await findAnnouncementService.GetForPageByIdAsync(id, ct);
    }
    
    /// <summary>
    /// Отменить объявление
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("cancel/{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelFindAnnouncementRM data,
        CancellationToken ct)
    {
        await deleteAnnouncementValidator.ValidateAndThrowAsync(data, ct);
        
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);

        var deleteDto = new DeleteFindAnnouncementData
        {
            AnnouncementId = id,
            DeleterId = userId,
            DeleteReason = (FindAnnouncementDeleteReason)data.CancelReason,
        };
        
        await findAnnouncementService.Cancel(deleteDto, ct);

        return NoContent();
    }
}