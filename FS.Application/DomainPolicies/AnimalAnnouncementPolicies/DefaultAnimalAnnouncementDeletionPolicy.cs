using FS.Core.Entities;
using FS.Core.Policies.AnnouncementPolicies;

namespace FS.Application.DomainPolicies.AnimalAnnouncementPolicies;

public class DefaultAnimalAnnouncementDeletionPolicy : IAnimalAnnouncementDeletionPolicy
{
    public required Guid UserId  { get; init; }
    public required AnimalAnnouncement Announcement  { get; init; }
    
    public bool CanDelete()
    {
        return UserId == Announcement.CreatorId;
    }
}