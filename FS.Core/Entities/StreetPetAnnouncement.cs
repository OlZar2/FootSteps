using FS.Core.Enums;
using FS.Core.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.Entities;

public class StreetPetAnnouncement : Announcement
{
    public Point Location { get; private set; }

    private StreetPetAnnouncement(Point location, Place fullPlace, PetType petType, Image[] images, User creator
        , District district)
        : base(fullPlace, petType, images, creator, AnnouncementType.Street, district)
    {
        Location = location;
    }

    public static StreetPetAnnouncement Create(
        Point location,
        Place place,
        PetType petType,
        Image[] images,
        User creator,
        District district)
    {
        return new StreetPetAnnouncement(location, place, petType, images, creator, district);
    }
    
    // EF
    private StreetPetAnnouncement(){}
}