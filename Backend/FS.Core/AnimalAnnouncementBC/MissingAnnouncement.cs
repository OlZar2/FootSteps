using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Entities;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Core.AnimalAnnouncementBC.Events;
using FS.Core.AnimalAnnouncementBC.Policies;
using FS.Core.Exceptions;
using FS.Core.ImageDomain.Entities;
using FS.Core.Shared.ValueObjects;
using NetTopologySuite.Geometries;

namespace FS.Core.AnimalAnnouncementBC;

public class MissingAnnouncement : PetAnnouncement
{
    private readonly List<SpottedLocation> _spottedLocations = [];
    
    public string PetName { get; private set; }
    
    public MissingAnnouncementDeleteReason DeleteReason { get; private set; }
    
    public IReadOnlyList<SpottedLocation> SpottedLocations => _spottedLocations;
    
    private MissingAnnouncement(
        string? street,
        string? house,
        List<FSImage> images,
        Guid creatorId,
        string? district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        bool isCompleted,
        Point location,
        string petName,
        DateTime createdAt,
        DateTime eventDate,
        string? description)
        : base(
            street: street,
            house: house,
            images,
            creatorId,
            district,
            petType,
            gender,
            color,
            breed,
            isCompleted,
            location,
            createdAt,
            eventDate,
            description)
    {
        PetName = petName;
    }

    public static MissingAnnouncement Create(
        string? street,
        string? house,
        List<FSImage> images,
        Guid creatorId,
        string? district,
        PetType petType,
        Gender gender,
        string? color,
        string? breed,
        Point location,
        string petName,
        DateTime eventDate,
        string? description)
    {
        var createdAt = DateTime.UtcNow;
        
        var created = new MissingAnnouncement(
            street: street,
            house: house,
            images,
            creatorId,
            district,
            petType,
            gender,
            color,
            breed,
            false,
            location,
            petName,
            createdAt,
            eventDate,
            description);
        
        created.AddDomainEvent(new MissingAnnouncementCreatedDomainEvent(created.Id));
        created.AddDomainEvent(new AnnouncementCreatedDomainEvent(created.Id));
        return created;
    }

    public void Cancel(MissingAnnouncementDeleteReason reason, IAnimalAnnouncementDeletionPolicy deletionPolicy)
    {
        if (!deletionPolicy.CanDelete())
            throw new NotEnoughRightsException(IssueCodes.AccessDenied,
                "Вы не явлвяетесь создателем объявления");

        if (IsDeleted)
            throw new DomainException(IssueCodes.Announcement.AlreadyCancelled, "Объявление уже отменено");

        IsDeleted = true;
        DeleteReason = reason;
    }

    public void ReportSpotted(
        Guid spottedUserId,
        CoordinatesVO location,
        List<FSImage> images)
    {
        var spottedLocation = SpottedLocation.Create(
            location: location,
            spottedUserId: spottedUserId,
            images: images,
            missingAnnouncementId: Id);
        
        _spottedLocations.Add(spottedLocation);
        
        AddDomainEvent(new ReportSpottedDomainEvent(
            MissingAnnouncementId: Id));
    }
    
    public void ReportFound(Guid foundUserId)
    {
        AddDomainEvent(new ReportFoundDomainEvent(
            AnnouncementId: Id,
            FoundUserId: foundUserId));
    }
    
    // EF
    private MissingAnnouncement(){}
}