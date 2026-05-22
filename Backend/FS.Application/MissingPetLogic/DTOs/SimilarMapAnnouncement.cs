using FS.Application.Shared.DTOs;

namespace FS.Application.MissingPetLogic.DTOs;

public record SimilarMapAnnouncement
{
    public required Guid Id { get; init; }
    public required CoordinatesDto Coordinates { get; init; }
    public required DateTime CreatedAt { get; init; }
}