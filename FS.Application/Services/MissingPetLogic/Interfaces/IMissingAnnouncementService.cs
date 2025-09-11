using FS.Application.DTOs.MissingAnnouncementDTOs;

namespace FS.Application.Services.MissingPetLogic.Interfaces;

public interface IMissingAnnouncementService
{
    Task<MissingAnnouncementFeed[]> GetFilteredMissingAnnouncementsByPageAsync(DateTime lastDateTime,
        AnnouncementFilter announcementFilter);
}