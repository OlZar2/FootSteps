using FS.Core.Enums;
using FS.Core.ValueObjects;

namespace FS.Core.Entities;

public abstract class Announcement
{
    public Guid Id { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public Place FullPlace { get; private set; }
    
    public District District { get; private set; }
    
    public PetType PetType { get; private set; }
    
    public Image[] Images  { get; private set; }
    
    public User Creator  { get; set; }
    
    public AnnouncementType Type { get; private set; }

    protected Announcement(Place fullPlace, PetType petType, Image[] images, User creator, AnnouncementType type,
        District district)
    {
        FullPlace = fullPlace;
        PetType = petType;
        Images = images;
        Creator = creator;
        Type = type;
        district = district;
    }
    
    // EF
    protected Announcement() { }
}