using FS.Application.DTOs.Shared;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public record SpottedInfo(Guid SpottedUserId, Guid AnnouncementId, CoordinatesDto Location) { }