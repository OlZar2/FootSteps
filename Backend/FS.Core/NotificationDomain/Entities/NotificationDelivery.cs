using FS.Core.Enums.Notifications;
using FS.Core.Shared.Abstractions;

namespace FS.Core.NotificationDomain.Entities;

public class NotificationDelivery : Entity
{
    public Guid NotificationId { get; private set; }
    
    public Guid? UserDeviceId { get; private set; }

    public string? RecipientEmail { get; private set; }
    
    public NotificationChannel Channel { get; private set; }
    
    public DeliveryStatus Status { get; private set; }
    
    public int AttemptCount { get; private set; }

    public DateTime CreatedAt { get; private set; }
    public DateTime? LastAttemptAt { get; private set; }
    public DateTime? SentAt { get; private set; }
    
    public DateTime? ReadAt { get; private set; }
    
    public string? Error { get; private set; }
    
    private NotificationDelivery(
        Guid notificationId,
        Guid? userDeviceId,
        string? recipientEmail,
        NotificationChannel channel,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        NotificationId = notificationId;
        UserDeviceId = userDeviceId;
        RecipientEmail = recipientEmail;
        Channel = channel;

        Status = DeliveryStatus.Pending;
        AttemptCount = 0;

        CreatedAt = DateTime.UtcNow;
    }

    public static NotificationDelivery Create(
        Guid notificationId,
        Guid? userDeviceId,
        NotificationChannel channel,
        Guid? id = null) 
    {
        if (notificationId == Guid.Empty)
            throw new ArgumentException("NotificationId is required.", nameof(notificationId));
        
        return new NotificationDelivery(notificationId, userDeviceId, null, channel, id);
    }

    public static NotificationDelivery CreateEmail(
        Guid notificationId,
        string recipientEmail,
        Guid? id = null)
    {
        if (notificationId == Guid.Empty)
            throw new ArgumentException("NotificationId is required.", nameof(notificationId));

        if (string.IsNullOrWhiteSpace(recipientEmail))
            throw new ArgumentException("RecipientEmail is required.", nameof(recipientEmail));

        return new NotificationDelivery(notificationId, null, recipientEmail, NotificationChannel.Email, id);
    }

    public void MarkAsSent()
    {
        Status = DeliveryStatus.Sent;
        SentAt = DateTime.UtcNow;
        LastAttemptAt = SentAt;
        AttemptCount++;
    }
    
    public void MarkAsFailed()
    {
        Status = AttemptCount > 10 ? DeliveryStatus.Stoped : DeliveryStatus.Failed;
        LastAttemptAt = DateTime.UtcNow;
        AttemptCount++;
    }
    
    public void MarkAsUnActual()
    {
        Status = DeliveryStatus.Unactual;
    }
    
    // EF
    private NotificationDelivery() { }
}
