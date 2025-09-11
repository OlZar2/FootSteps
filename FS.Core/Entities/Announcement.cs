using FS.Core.ValueObjects;

namespace FS.Core.Entities;

public class Announcement
{
    public Guid Id { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public Place Place { get; private set; }
    
    public Guid PetTypeId { get; private set; }
    public PetType PetType { get; private set; }
    
    public Image[] Images  { get; private set; }
    
    public User Creator  { get; set; }

    protected Announcement(Place place, PetType petType, Image[] images, User creator)
    {
        Place = place;
        PetType = petType;
        Images = images;
        Creator = creator;
    }
    
    // EF
    protected Announcement() { }
}