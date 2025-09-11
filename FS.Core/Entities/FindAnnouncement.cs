using FS.Core.Enums;
using FS.Core.ValueObjects;

namespace FS.Core.Entities;

public class FindAnnouncement : Announcement
{
    public Gender? Gender { get; private set; }
    
    public bool IsCompleted { get; private set; }

    private FindAnnouncement(Gender? gender, Place fullPlace, PetType petType, Image[] images, User creator,
        bool isCompleted, District district)
        : base(fullPlace, petType, images, creator, AnnouncementType.Find, district)
    {
        Gender = gender;
        Creator = creator;
    }

    public static FindAnnouncement Create(Gender? gender, Place place, PetType petType, Image[] images, User creator,
        District district)
    {
        return new FindAnnouncement(gender, place, petType, images, creator, false, district);
    }
    
    //EF
    private FindAnnouncement() { }
}