using System.Security.Claims;
using FluentValidation;
using FS.API.Errors;
using FS.API.RequestsModels.FindAnnouncements;
using FS.API.RequestsModels.MissingAnnouncements;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application.DTOs.FindAnnouncementDTOs;
using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.DTOs.Shared;
using FS.Application.Services.FindAnnouncementLogic.Implementations;
using FS.Application.Services.FindAnnouncementLogic.Interfaces;
using FS.Contracts.Error;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

[ApiController]
[Route("api/find-announcement")]
public class FindAnnouncementsController(
    IFindAnnouncementService findAnnouncementService,
    IValidator<CreateFindAnnouncementRM> createAnnouncementValidator,
    IValidator<DeleteFindAnnouncementRM> deleteAnnouncementValidator,
    IClaimService claimService) : ControllerBase
{
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
            Gender = data.Gender!.Value,
            PetType = data.PetType!.Value,
            EventDate = data.EventDate!.Value,
            Description = data.Description,
        };
        
        var response = await findAnnouncementService.Create(createDTO, ct);

        return response;
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MissingAnnouncementPage), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<FindAnnouncementPage> GetForPage(Guid id, CancellationToken ct)
    {
        return await findAnnouncementService.GetForPageByIdAsync(id, ct);
    }
    
    [Authorize]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromBody] DeleteFindAnnouncementRM data,
        CancellationToken ct)
    {
        await deleteAnnouncementValidator.ValidateAndThrowAsync(data, ct);
        
        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);

        var deleteDto = new DeleteFindAnnouncementData
        {
            AnnouncementId = id,
            DeleterId = userId,
            DeleteReason = data.DeleteReason!.Value,
        };
        
        await findAnnouncementService.Delete(deleteDto, ct);

        return NoContent();
    }
}