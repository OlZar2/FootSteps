namespace FS.Application.NotificationLogic.DTOs;

public record EmailNotificationDto
{
    public required string RecipientEmail { get; init; }
    public required string Subject { get; init; }
    public required string Body { get; init; }
}
