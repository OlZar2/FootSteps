using FS.Core.ImageDomain.Entities;
using FS.Core.Shared.Abstractions;

namespace FS.Core.AnimalAnnouncementBC.Entities;

public class FoundReport : Entity
{
    private readonly List<FSImage> _images = [];
    
    public Guid? FoundUserId { get; private set; }
    
    public Guid MissingAnnouncementId { get; private set; }
    
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    
    public IReadOnlyList<FSImage> Images => _images.AsReadOnly();
    
    private FoundReport(
        Guid foundUserId,
        Guid missingAnnouncementId,
        List<FSImage> images,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        FoundUserId = foundUserId;
        MissingAnnouncementId = missingAnnouncementId;
        _images = images;
    }

    public static FoundReport Create(
        Guid foundUserId,
        Guid missingAnnouncementId,
        List<FSImage> images,
        Guid? id = null)
    {
        return new FoundReport(foundUserId, missingAnnouncementId, images, id);
    }

    // EF
    private FoundReport() { }
}