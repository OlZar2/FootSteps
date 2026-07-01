using System.ComponentModel.DataAnnotations.Schema;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.Enums;
using FS.Core.ImageDomain.Entities;
using FS.Core.SearchDomain.Events;
using FS.Core.Shared.Abstractions;
using NetTopologySuite.Geometries;
using Pgvector;

namespace FS.Core.SearchDomain;

public class SearchRequest : AggregateRoot
{
    public Guid CreatorId { get; private set; }
    
    public Guid ImageId { get; private set; }
    public FSImage Image { get; private set; }
    public SearchRequestStatus SearchRequestStatus  { get; private set; }
    
    [Column(TypeName = "vector(512)")]
    public Vector? Embedding { get; set; }
    
    public Point Location { get; private set; }
    
    public string? ErrorCode { get; private set; }
    
    public List<AnimalAnnouncement> Results { get; private set; } = [];
    public DateTime CreatedAt { get; private set; }

    private SearchRequest(
        FSImage image,
        SearchRequestStatus searchRequestStatus,
        Guid creatorId,
        Point location,
        Guid? id = null)
    : base(id ?? Guid.NewGuid())
    {
        Image = image;
        ImageId = image.Id;
        SearchRequestStatus = searchRequestStatus;
        CreatorId = creatorId;
        Location = location;
        CreatedAt = DateTime.UtcNow;
    }

    public static SearchRequest Create(FSImage image, Guid creatorId, Point location, Guid? id = null)
    {
        var created = new SearchRequest(image, SearchRequestStatus.Queued, creatorId, location, id);
        
        created.AddDomainEvent(new SearchRequestCreatedDomainEvent(created.Id));

        return created;
    }

    public void SetResults(List<AnimalAnnouncement> results)
    {
        Results = results;
        SearchRequestStatus = SearchRequestStatus.Success;
        
        AddDomainEvent(new SearchRequestCompletedDomainEvent(Id));
    }
    
    public void SetEmbedding(Vector embedding)
    {
        SearchRequestStatus = SearchRequestStatus.Processing;
        Embedding = embedding;
    }
    
    public void SetError(string errorCode)
    {
        ErrorCode = errorCode;
        SearchRequestStatus = SearchRequestStatus.Error;
        
        AddDomainEvent(new SearchRequestCompletedWithErrorDomainEvent(Id));
    }

    // EF
    private SearchRequest(){}
}