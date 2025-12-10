using FS.Application.DTOs.Shared;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public record MissingAnnouncementForNotifyData
{
    public required CoordinatesDto Coordinates { get; init; }
    public required Guid CreatorId { get; init; }
}