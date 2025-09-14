using System.Security.Claims;
using FluentValidation;
using FS.API.Errors;
using FS.API.RequestsModels.MissingAnnouncements;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Services.MissingPetLogic.Interfaces;
using FS.Contracts.Error;
using FS.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

[ApiController]
[Route("api/missing-announcement")]
public class MissingAnnouncementController(
    IMissingAnnouncementService missingAnnouncementService,
    IClaimService claimService,
    IValidator<CreateMissingAnnouncementRM> createAnnouncementValidator,
    IValidator<DeleteMissingAnnouncementRM> deleteAnnouncementValidator) : ControllerBase
{
    /// <summary>
    /// Возвращает список объявлений о пропаже.
    /// </summary>
    /// <param name="lastDateTime">Дата и время последнего полученного объявления.</param>
    /// <param name="filter">Фильтр объявлений (например, по типу, категории и т. д.).</param>
    /// <param name="ct">Токен отмены.</param>
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
    
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(CreatedMissingAnnouncement), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<CreatedMissingAnnouncement> Create(
        [FromForm] CreateMissingAnnouncementRM data,
        CancellationToken ct)
    {
        await createAnnouncementValidator.ValidateAndThrowAsync(data, ct);
        
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);
        
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
            FullPlace = data.FullPlace,
            District = data.District,
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
        
        var response = await missingAnnouncementService.Create(createDTO, ct);

        return response;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MissingAnnouncementPage), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<MissingAnnouncementPage> GetForPage(Guid id, CancellationToken ct)
    {
        return await missingAnnouncementService.GetForPageByIdAsync(id, ct);
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromBody] DeleteMissingAnnouncementRM data,
        CancellationToken ct)
    {
        await deleteAnnouncementValidator.ValidateAndThrowAsync(data, ct);
        
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);

        var deleteDto = new DeleteMissingAnnouncementData
        {
            AnnouncementId = id,
            DeleterId = userId,
            DeleteReason = data.DeleteReason!.Value,
        };
        
        await missingAnnouncementService.Delete(deleteDto, ct);

        return NoContent();
    }
}