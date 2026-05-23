using FS.Core.ImageDomain.Entities;
using FS.Core.Shared.Abstractions;
using FS.Core.Shared.ValueObjects;

namespace FS.Core.AnimalAnnouncementBC.Entities;

public class SpottedLocation : Entity
{
    private readonly List<FSImage> _images = [];
    
    public CoordinatesVO Location { get; private set; }
    
    public Guid? SpottedUserId { get; private set; }
    
    public Guid MissingAnnouncementId { get; private set; }
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    public IReadOnlyList<FSImage> Images => _images.AsReadOnly();

    private SpottedLocation(
        CoordinatesVO location,
        Guid? spottedUserId,
        Guid missingAnnouncementId,
        List<FSImage> images,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Location = location;
        SpottedUserId = spottedUserId;
        MissingAnnouncementId = missingAnnouncementId;
        _images = images;
    }

    public static SpottedLocation Create(
        CoordinatesVO location,
        Guid? spottedUserId,
        Guid missingAnnouncementId,
        List<FSImage> images,
        Guid? id = null)
    {
        return new SpottedLocation(location, spottedUserId, missingAnnouncementId, images, id);
    }

    // EF
    private SpottedLocation()
    {
        
    }
}