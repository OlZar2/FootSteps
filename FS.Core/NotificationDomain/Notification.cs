using FS.Contracts.Error;
using FS.Core.Enums.Notifications;
using FS.Core.Exceptions;
using FS.Core.NotificationDomain.Entities;
using FS.Core.Shared.Abstractions;

namespace FS.Core.NotificationDomain;

public class Notification : AggregateRoot
{
    private readonly List<NotificationDelivery> _notificationDeliveries = [];
    public NotificationType Type { get; private set; }

    public string Subject { get; private set; } = string.Empty;
    public string Text { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }
    
    public Guid? TargetEntityId { get; private set; }
    
    public bool IsCompleted {get; private set;} = false;
    
    public NotificationChannel[] Channels { get; private set; } 

    public IReadOnlyCollection<NotificationDelivery> NotificationDeliveries => _notificationDeliveries.AsReadOnly();
    
    private Notification(
        NotificationType type,
        string subject,
        string text,
        Guid? targetEntityId,
        NotificationChannel[] channels,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Subject = subject;

        Text = string.IsNullOrWhiteSpace(text)
            ? throw new ArgumentException("Notification text is required.", nameof(text))
            : text;
        
        Type = type;
        
        TargetEntityId = targetEntityId;
        Channels = channels;
        
        CreatedAt = DateTime.UtcNow;
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
            channels: channels,
            targetEntityId: targetEntityId);
    }

    public void SetDeliveries(NotificationDelivery[] deliveries)
    {
        _notificationDeliveries.AddRange(deliveries);
    }

    public void MarkAsCompleted()
    {
        IsCompleted = true;
    }
    
    public void MarkDeliveryAsSent(Guid deliveryId)
    {
        var delivery = GetDelivery(deliveryId);
        delivery.MarkAsSent();
    }
    
    public void MarkDeliveryAsFailed(Guid deliveryId)
    {
        var delivery = GetDelivery(deliveryId);
        delivery.MarkAsFailed();
    }
    
    private NotificationDelivery GetDelivery(Guid deliveryId)
    {
        return _notificationDeliveries.FirstOrDefault(d => d.Id == deliveryId)
               ?? throw new DomainException(IssueCodes.NotFound, $"notification delivery by id {deliveryId} not found.");
    }
    
    // EF
    private Notification() { }
}