using FS.Core.Enums;
using FS.Core.ValueObjects;

namespace FS.Core.Entities;

public class FindAnnouncement : Announcement
{
    public Gender? Gender { get; set; }

    private FindAnnouncement(Gender? gender, Place place, PetType petType, Image[] images, User creator)
        : base(place, petType, images, creator)
    {
        Gender = gender;
        Creator = creator;
    }

    public static FindAnnouncement Create(Gender? gender, Place place, PetType petType, Image[] images, User creator)
    {
        return new FindAnnouncement(gender, place, petType, images, creator);
    }
    
    //EF
    private FindAnnouncement() { }
}