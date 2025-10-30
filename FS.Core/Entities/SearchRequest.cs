using System.ComponentModel.DataAnnotations.Schema;
using FS.Core.Enums;
using Pgvector;

namespace FS.Core.Entities;

public class SearchRequest
{
    public Guid Id { get; private set; }
    
    public Guid CreatorId { get; private set; }
    public string ImagePath { get; private set; }
    public SearchRequestStatus SearchRequestStatus  { get; private set; }
    
    [Column(TypeName = "vector(512)")]
    public Vector? Embedding { get; set; }
    
    public List<AnimalAnnouncement> Results { get; private set; } = [];
    public DateTime CreatedAt { get; private set; }

    private SearchRequest(string imagePath, SearchRequestStatus searchRequestStatus, Guid creatorId)
    {
        ImagePath = imagePath;
        SearchRequestStatus = searchRequestStatus;
        CreatorId = creatorId;
        CreatedAt = DateTime.UtcNow;
    }

    public static SearchRequest Create(string imagePath, Guid creatorId)
    {
        if (string.IsNullOrWhiteSpace(imagePath))
            throw new ArgumentNullException(nameof(imagePath));

        return new SearchRequest(imagePath, SearchRequestStatus.Queued, creatorId);
    }

    public void SetResults(List<AnimalAnnouncement> results)
    {
        Results = results;
    }
}