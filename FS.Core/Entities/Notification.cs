using FS.Core.Abstractions;
using FS.Core.Enums.Notifications;

namespace FS.Core.Entities;

public class Notification : AggregateRoot
{
    public NotificationType Type { get; private set; }

    public string Subject { get; private set; } = string.Empty;
    public string Text { get; private set; } = null!;

    public DateTimeOffset CreatedAt { get; private set; }
    
    public Guid? TargetEntityId { get; private set; }
    
    public bool IsCompleted {get; private set;} = false;

    private Notification(
        NotificationType type,
        string subject,
        string text,
        Guid? targetEntityId,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Subject = subject;

        Text = string.IsNullOrWhiteSpace(text)
            ? throw new ArgumentException("Notification text is required.", nameof(text))
            : text;
        
        Type = type;
        
        TargetEntityId = targetEntityId;

        CreatedAt = DateTimeOffset.UtcNow;
    }
    
    public static Notification Create(
        NotificationType type,
        string subject,
        string text,
        IEnumerable<NotificationChannel> allowedChannels,
        Guid? targetEntityId = null)
    {
        if (allowedChannels is null)
            throw new ArgumentNullException(nameof(allowedChannels));

        var channels = allowedChannels.ToArray();
        if (channels.Length == 0)
            throw new ArgumentException("AllowedChannels must contain at least one channel.", nameof(allowedChannels));

        return new Notification(
            type: type,
            subject: subject,
            text: text,
            targetEntityId: targetEntityId);
    }

    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }
    
    // EF
    private Notification() { }
}