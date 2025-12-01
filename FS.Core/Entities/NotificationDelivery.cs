using FS.Core.Abstractions;
using FS.Core.Enums.Notifications;

namespace FS.Core.Entities;

public class NotificationDelivery : AggregateRoot
{
    public Guid NotificationId { get; private set; }
    
    public Guid? UserDeviceId { get; private set; }
    public UserDevice UserDevice { get; private set; }
    
    public NotificationChannel Channel { get; private set; }
    
    public DeliveryStatus Status { get; private set; }
    
    public int AttemptCount { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? LastAttemptAt { get; private set; }
    public DateTimeOffset? SentAt { get; private set; }
    
    public DateTimeOffset? ReadAt { get; private set; }
    
    public string? Error { get; private set; }
    
    private NotificationDelivery(
        Guid notificationId,
        UserDevice userDevice,
        NotificationChannel channel,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        NotificationId = notificationId;
        UserDevice = userDevice;
        UserDeviceId = userDevice.Id;
        Channel = channel;

        Status = DeliveryStatus.Pending;
        AttemptCount = 0;

        CreatedAt = DateTime.UtcNow;
    }

    public static NotificationDelivery Create(
        Guid notificationId,
        UserDevice userDevice,
        NotificationChannel channel)
    {
        if (notificationId == Guid.Empty)
            throw new ArgumentException("NotificationId is required.", nameof(notificationId));
        
        return new NotificationDelivery(notificationId, userDevice, channel);
    }

    public void MarkAsSent()
    {
        Status = DeliveryStatus.Sent;
        SentAt = DateTime.UtcNow;
        LastAttemptAt = SentAt;
    }
    
    public void MarkAsFailed()
    {
        if (AttemptCount > 10)
        {
            Status = DeliveryStatus.Stoped;
        }
        Status = DeliveryStatus.Failed;
        LastAttemptAt = DateTime.UtcNow;
        AttemptCount++;
    }
    
    // EF
    private NotificationDelivery() { }
}