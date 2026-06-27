using FS.Core.Enums.Notifications;

namespace FS.Application.NotificationLogic.DTOs;

public record EmailNotificationDto
{
    public required NotificationType Type { get; init; }
    public Guid? TargetEntityId { get; init; }
    public string? EmailConfirmationToken { get; init; }
    public required string RecipientEmail { get; init; }
    public required string Subject { get; init; }
    public required string Body { get; init; }
}
