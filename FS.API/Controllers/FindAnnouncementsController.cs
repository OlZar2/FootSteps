using System.Security.Claims;
using FluentValidation;
using FS.API.Errors;
using FS.API.RequestsModels.FindAnnouncements;
using FS.API.Services.ClaimLogic.Interfaces;
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
    IClaimService claimService) : ControllerBase
{
    /// <summary>
    /// Возвращает список из 20 объялений о пропаже
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
    /// Создать объявление о пропаже
    /// </summary>
    /// <param name="data"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CreatedFindAnnouncement), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<CreatedFindAnnouncement> Create(
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

        var createDTO = new CreateFindAnnouncementData
        {
            FullPlace = data.FullPlace,
            District = data.District,
            Location = data.Location,
            Images = fileInfos,
            CreatorId = userId,
            Breed = data.Breed,
            Color = data.Color,
            Gender = (Gender)data.Gender,
            PetType = (PetType)data.PetType,
            EventDate = data.EventDate!.Value,
            Description = data.Description,
        };
        
        var response = await findAnnouncementService.Create(createDTO, ct);

        return response;
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