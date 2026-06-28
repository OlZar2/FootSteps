using FS.Application.UserLogic.DTOs;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Persistence.Projections.Shared;
using NetTopologySuite.Geometries;

namespace FS.Persistence.Projections.MissingAnnouncement;

public class MissingAnnouncementPageProjection
{
    public Guid Id { get; init; }
    public string? Street { get; init; }
    public string? District { get; init; }
    public string? House { get; init; }
    public string[] ImagesPaths { get; init; } = [];
    public AnnouncementCreator Creator { get; init; } = null!;
    public PetType PetType { get; init; }
    public Gender Gender { get; init; }
    public string? Breed { get; init; }
    public string? Color { get; init; }
    public AnnouncementType Type { get; init; }
    public Point Location { get; init; } = null!;
    public DateTime EventDate { get; init; }
    public string? Description { get; init; }
    public required string PetName { get; init; }
    public DeleteType? DeleteType { get; init; }
    public SimilarMapAnnouncementProjection[] SimilarAnnouncements { get; init; } = [];
}
