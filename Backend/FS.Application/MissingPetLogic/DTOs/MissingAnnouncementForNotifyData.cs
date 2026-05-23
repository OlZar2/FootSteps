using FS.Application.Shared.DTOs;

namespace FS.Application.MissingPetLogic.DTOs;

public record MissingAnnouncementForNotifyData
{
    public required CoordinatesDto Coordinates { get; init; }
    public required Guid CreatorId { get; init; }
}