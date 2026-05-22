using FS.Application.Shared.DTOs;

namespace FS.Application.MissingPetLogic.DTOs;

public record SpottedInfo(Guid SpottedUserId, Guid AnnouncementId, CoordinatesDto Location, FileData[] Images) { }