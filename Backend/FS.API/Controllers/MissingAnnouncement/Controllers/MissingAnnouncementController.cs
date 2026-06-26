using System.Security.Claims;
using FluentValidation;
using FS.API.Controllers.MissingAnnouncement.RequestModels;
using FS.API.Errors;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.API.Services.GeoLogic.Interfaces;
using FS.Application.MissingPetLogic.DTOs;
using FS.Application.MissingPetLogic.Interfaces;
using FS.Application.Shared.DTOs;
using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers.MissingAnnouncement.Controllers;

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
    IValidator<AnnouncementFilter> filterValidator,
    IValidator<ReportSpottedRM> reportSpottedValidator,
    IValidator<ReportFoundRM> reportFoundValidator,
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
        [FromQuery] DateTime? lastDateTime,
        [FromQuery] AnnouncementFilter filter,
        CancellationToken ct)
    {
        await filterValidator.ValidateAndThrowAsync(filter, ct);
        
        var feed = await missingAnnouncementService.GetFeedAsync(
            filter,
            lastDateTime,
            ct);

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

        var house = await geocoder.GetHouseOrNull(data.Location, ct);
        var street = await geocoder.GetStreetOrNull(data.Location, ct);
        var district = await geocoder.GetDistrictOrNull(data.Location, ct)
                       ?? await geocoder.GetLocalityOrNull(data.Location, ct);
        
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

    /// <summary>
    /// Возвращает объявления о пропаже текущего пользователя
    /// </summary>
    /// <param name="lastDateTime">Дата и время последнего полученного обяъвления(для пагинации)</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [Authorize]
    [HttpGet("me/feed")]
    [ProducesResponseType(typeof(MyAnnouncementFeed[]), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<MyAnnouncementFeed[]> GetUserFeed([FromQuery] DateTime? lastDateTime, CancellationToken ct)
    {
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);

        return await missingAnnouncementService.GetFeedItemsByCreatorByPage(userId, lastDateTime, ct);
    }

    /// <summary>
    /// Метод для сообщения о нахождении потервшегося питомца
    /// </summary>
    /// <param name="announcementId">id объявления о пропаже</param>
    /// <param name="request">данные запроса</param>
    /// <param name="ct"></param>
    [HttpPost("{announcementId:guid}/report-found")]
    [Authorize]
    public async Task ReportFound(Guid announcementId, [FromForm] ReportFoundRM request, CancellationToken ct)
    {
        await reportFoundValidator.ValidateAndThrowAsync(request, ct);
        
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

        var foundInfo = new FoundInfo(
            FoundUserId: userId,
            AnnouncementId: announcementId,
            fileInfos);
        
        await missingAnnouncementService.ReportFoundAsync(foundInfo, ct);
    }
    
    /// <summary>
    /// Метод для сообщения о том, что питомца заметили
    /// </summary>
    /// <param name="announcementId">id объявления о пропаже</param>
    /// <param name="request">данные запроса</param>
    /// <param name="ct"></param>
    [HttpPost("{announcementId:guid}/report-spotted")]
    [Authorize]
    public async Task ReportSpotted(Guid announcementId, [FromForm] ReportSpottedRM request, CancellationToken ct)
    {
        await reportSpottedValidator.ValidateAndThrowAsync(request, ct);
        
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
        
        var spottedInfo = new SpottedInfo(
            SpottedUserId: userId,
            AnnouncementId: announcementId,
            Location: request.Coordinates,
            Images: fileInfos);
        
        await missingAnnouncementService.ReportSpottedAsync(spottedInfo, ct);
    }
    
    /// <summary>
    /// Возвращает список точек, где замечали питомца
    /// </summary>
    /// <param name="announcementId">id объявления о пропаже</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("{announcementId:guid}/spotted-locations")]
    public async Task<SpottedLocationDto[]> GetSpottedLocations(Guid announcementId, CancellationToken ct) =>
        await missingAnnouncementService.GetSpottedLocations(announcementId, ct);
    
    /// <summary>
    /// Возвращает список сообщений о находке питомца
    /// </summary>
    /// <param name="announcementId">id объявления о пропаже</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpGet("{announcementId:guid}/found-reports")]
    public async Task<FoundReportDto[]> GetFoundReports(Guid announcementId, CancellationToken ct) =>
        await missingAnnouncementService.GetFoundReports(announcementId, ct);
}