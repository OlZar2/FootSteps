using FS.Contracts.Error;
using FS.Core.AnimalAnnouncementBC.Enums;
using FS.Core.Exceptions;
using FS.Core.ImageDomain.Entities;
using FS.Core.Shared.Abstractions;
using FS.Core.Shared.ValueObjects;
using NetTopologySuite.Geometries;
using Pgvector;

namespace FS.Core.AnimalAnnouncementBC;

public abstract class AnimalAnnouncement : AggregateRoot
{
    protected readonly List<FSImage> _images = [];
    
    public DateTime CreatedAt { get; private set; }
    
    public string? Street { get; private set; }
    
    public string? House { get; private set; }
    
    public string? District { get; private set; }
    
    public IReadOnlyCollection<FSImage> Images => _images.AsReadOnly();
    
    public Guid CreatorId  { get; set; }
    
    public PetType PetType { get; private set; }
    
    public AnnouncementType Type { get; private set; }
    
    public Point Location { get; private set; }
    
    public DateTime EventDate { get; private set; }
    
    public DeleteType? DeleteType { get; protected set; }

    public int ReportCount { get; private set; }

    protected AnimalAnnouncement(
        List<FSImage> images,
        Guid creatorId,
        string? district,
        string? street,
        string? house,
        PetType petType,
        Point location,
        DateTime createdAt,
        DateTime eventDate,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        
        Street = street;
        House = house;
        _images = images;
        CreatorId = creatorId;
        District = district;
        PetType = petType;
        Location = location;
        CreatedAt = createdAt;
        EventDate = eventDate;
    }

    public void UpdateImageEmbedding(Guid imageId, Vector vector)
    {
        var image = _images.FirstOrDefault(i => i.Id == imageId)
            ?? throw new DomainException(IssueCodes.NotFound, $"image by id {imageId} not found.");
        image.SetEmbedding(vector);
    }

    public void HideByAdmin()
    {
        DeleteType = Enums.DeleteType.AdminHide;
    }

    protected void CancelByUser()
    {
        if (DeleteType == Enums.DeleteType.AdminHide)
            throw new DomainException(
                IssueCodes.Announcement.DeletedByAdmin,
                "Объявление удалено по причинам модерации");

        if (DeleteType == Enums.DeleteType.UserCancel)
            throw new DomainException(IssueCodes.Announcement.AlreadyCancelled, "Объявление уже отменено");

        DeleteType = Enums.DeleteType.UserCancel;
    }
    
    protected virtual void OnImageEmbeddingUpdated(Guid imageId)
    {
        // базовое поведение: ничего не делаем
    }
    
    // EF
    protected AnimalAnnouncement() { }
}
