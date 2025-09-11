using FS.Core.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public class StreetPetAnnouncement : Announcement
{
    public Point Location { get; private set; }

    private StreetPetAnnouncement(Point location, Place place, PetType petType, Image[] images, User creator)
        : base(place, petType, images, creator)
    {
        Location = location;
    }

    public static StreetPetAnnouncement Create(
        Point location,
        Place place,
        PetType petType,
        Image[] images,
        User creator)
    {
        return new StreetPetAnnouncement(location, place, petType, images, creator);
    }
    
    // EF
    private StreetPetAnnouncement(){}
}