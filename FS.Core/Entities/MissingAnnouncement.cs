using FS.Core.Enums;
using FS.Core.ValueObjects;

namespace FS.Core.Entities;

public class MissingAnnouncement : Announcement
{
    public string PetName { get; set; }
    
    public Gender Gender { get; set; }

    private MissingAnnouncement(
        string petName,
        Gender gender,
        Place place,
        PetType petType,
        Image[] images,
        User creator) : base(place, petType, images, creator)
    {
        PetName = petName;
        Gender = gender;
    }

    public static MissingAnnouncement Create(
        string petName,
        Gender gender,
        Place place,
        PetType petType,
        Image[] images,
        User creator)
    {
        return new MissingAnnouncement(petName, gender, place, petType, images, creator);
    }
    
    // EF
    private MissingAnnouncement(){}
}