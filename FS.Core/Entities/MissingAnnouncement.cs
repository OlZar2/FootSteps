using FS.Core.Enums;
using FS.Core.ValueObjects;

namespace FS.Core.Entities;

public class MissingAnnouncement : Announcement
{
    public string PetName { get; private set; }
    
    public Gender Gender { get; private set; }
    
    public bool IsCompleted { get; private set; }

    private MissingAnnouncement(
        string petName,
        Gender gender,
        Place fullPlace,
        PetType petType,
        Image[] images,
        User creator,
        bool isCompleted,
        District district) : base(fullPlace, petType, images, creator, AnnouncementType.Missing, district)
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
        User creator,
        District district)
    {
        return new MissingAnnouncement(petName, gender, place, petType, images, creator, false, district);
    }
    
    // EF
    private MissingAnnouncement(){}
}