using System.Security.Claims;
using FluentValidation;
using FS.API.Controllers.AnimalAnnouncement.RequestModels;
using FS.API.Errors;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application.AnnouncementLogic.Interfaces;
using FS.Application.AnnouncementReportLogic.DTOs;
using FS.Application.AnnouncementReportLogic.Interfaces;
using FS.Contracts.Error;
using FS.Core.UserDomain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace FS.API.Controllers.AnimalAnnouncement.Controllers;

/// <summary>
/// Методы для работы с объявлениями
/// </summary>
[ApiController]
[Route("api/animal-announcement")]
public class AnimalAnnouncementController(
    IAnimalAnnouncementService animalAnnouncementService,
    IAnnouncementReportService announcementReportService,
    IClaimService claimService,
    IValidator<ReportAnnouncementRM> reportAnnouncementValidator) : ControllerBase
{
    [Authorize(Roles = nameof(Role.Admin))]
    [HttpPost("{announcementId:guid}/admin-hide")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> HideByAdmin(Guid announcementId, CancellationToken ct)
    {
        await animalAnnouncementService.HideByAdminAsync(announcementId, ct);

        return NoContent();
    }

    /// <summary>
    /// Пожаловаться на объявление
    /// </summary>
    /// <param name="announcementId">Id объявления</param>
    /// <param name="request">Данные жалобы</param>
    /// <param name="ct"></param>
    [Authorize]
    [HttpPost("{announcementId:guid}/report")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorEnvelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Report(
        Guid announcementId,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] ReportAnnouncementRM? request,
        CancellationToken ct)
    {
        var reportRequest = request ?? new ReportAnnouncementRM();
        await reportAnnouncementValidator.ValidateAndThrowAsync(reportRequest, ct);

        var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var userId = claimService.TryParseGuidClaim(userIdClaim);

        var data = new ReportAnnouncementData
        {
            AnnouncementId = announcementId,
            ReporterId = userId,
            Comment = reportRequest.Comment,
        };

        await announcementReportService.ReportAsync(data, ct);

        return NoContent();
    }
}
