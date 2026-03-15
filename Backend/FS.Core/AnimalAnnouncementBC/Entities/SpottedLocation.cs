using FS.Core.Shared.Abstractions;
using FS.Core.Shared.ValueObjects;

namespace FS.Core.AnimalAnnouncementBC.Entities;

public class SpottedLocation : Entity
{
    public CoordinatesVO Location { get; private set; }
    
    public Guid? SpottedUserId { get; private set; }
    
    public Guid MissingAnnouncementId { get; private set; }
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    private SpottedLocation(
        CoordinatesVO location,
        Guid? spottedUserId,
        Guid missingAnnouncementId,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Location = location;
        SpottedUserId = spottedUserId;
        MissingAnnouncementId = missingAnnouncementId;
    }

    public static SpottedLocation Create(
        CoordinatesVO location,
        Guid? spottedUserId,
        Guid missingAnnouncementId,
        Guid? id = null)
    {
        return new SpottedLocation(location, spottedUserId, missingAnnouncementId, id);
    }

    // EF
    private SpottedLocation()
    {
        
    }
}