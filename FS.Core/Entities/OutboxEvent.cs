namespace FS.Core.Entities;

public class OutboxEvent
{
    public Guid Id { get; private set; }
    public DateTime CreatedUtc { get; private set; } = DateTime.UtcNow;
    public string Payload { get; private set; } = string.Empty;
    public string Type { get; private set; } = string.Empty;
    public DateTime? PublishedUtc { get; private set; }

    public OutboxEvent(string type, string payload)
    {
        Type = type;
        Payload = payload;
    }
    
    public static OutboxEvent Create(string type, string payload)
    {
        if (string.IsNullOrWhiteSpace(type))
            throw new ArgumentNullException(nameof(type));
        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentNullException(nameof(payload));
        
        return new OutboxEvent(type, payload);
    }

    public void MarkAsPublished()
    {
        PublishedUtc = DateTime.UtcNow;
    }
    
    // EF
    private OutboxEvent(){}
}