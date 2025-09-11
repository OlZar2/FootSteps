using FS.Application.DTOs.MissingAnnouncementDTOs;
using FS.Application.Services.MissingPetLogic.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FS.API.Controllers;

[ApiController]
[Route("api/missingAnnouncement")]
public class MissingAnnouncementController(IMissingAnnouncementService missingAnnouncementService) : ControllerBase
{
    public async Task<MissingAnnouncementFeed[]> GetMissingAnnouncement(
        DateTime lastDateTime,
        AnnouncementFilter filter)
    {
        var feed = await missingAnnouncementService
            .GetFilteredMissingAnnouncementsByPageAsync(lastDateTime, filter);

        return feed;
    }
}