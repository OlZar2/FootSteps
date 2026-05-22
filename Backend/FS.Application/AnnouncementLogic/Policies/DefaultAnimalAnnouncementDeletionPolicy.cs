using FS.Core.AnimalAnnouncementBC;
using FS.Core.AnimalAnnouncementBC.Policies;

namespace FS.Application.AnnouncementLogic.Policies;

public class DefaultAnimalAnnouncementDeletionPolicy : IAnimalAnnouncementDeletionPolicy
{
    public required Guid UserId  { get; init; }
    public required AnimalAnnouncement Announcement  { get; init; }
    
    public bool CanDelete()
    {
        return UserId == Announcement.CreatorId;
    }
}