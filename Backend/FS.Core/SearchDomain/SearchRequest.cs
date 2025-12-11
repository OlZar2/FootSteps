using System.ComponentModel.DataAnnotations.Schema;
using FS.Core.AnimalAnnouncementBC;
using FS.Core.Enums;
using FS.Core.ImageDomain.Entities;
using FS.Core.SearchDomain.Events;
using FS.Core.Shared.Abstractions;
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
    
    public List<AnimalAnnouncement> Results { get; private set; } = [];
    public DateTime CreatedAt { get; private set; }

    private SearchRequest(FSImage image, SearchRequestStatus searchRequestStatus, Guid creatorId, Guid? id = null)
    : base(id ?? Guid.NewGuid())
    {
        Image = image;
        ImageId = image.Id;
        SearchRequestStatus = searchRequestStatus;
        CreatorId = creatorId;
        CreatedAt = DateTime.UtcNow;
    }

    public static SearchRequest Create(FSImage image, Guid creatorId, Guid? id = null)
    {
        var created = new SearchRequest(image, SearchRequestStatus.Queued, creatorId, id);
        
        created.AddDomainEvent(new SearchRequestCreatedDomainEvent(created.Id));

        return created;
    }

    public void SetResults(List<AnimalAnnouncement> results)
    {
        Results = results;
    }
    
    public void SetEmbedding(Vector embedding)
    {
        Embedding = embedding;
    }

    // EF
    private SearchRequest(){}
}