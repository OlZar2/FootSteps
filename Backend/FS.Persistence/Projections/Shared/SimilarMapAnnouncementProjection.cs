using NetTopologySuite.Geometries;

namespace FS.Persistence.Projections.Shared;

public class SimilarMapAnnouncementProjection
{
    public Guid Id { get; init; }
    public Point Location { get; init; } = null!;
    public DateTime CreatedAt { get; init; }
}