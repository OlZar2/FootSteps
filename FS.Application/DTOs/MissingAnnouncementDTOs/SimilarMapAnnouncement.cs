using FS.Application.DTOs.Shared;

namespace FS.Application.DTOs.MissingAnnouncementDTOs;

public record SimilarMapAnnouncement
{
    public required Guid Id { get; init; }
    public required CoordinatesDto Coordinates { get; init; }
    public required DateTime CreatedAt { get; init; }
}