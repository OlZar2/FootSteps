namespace FS.Application.DTOs.Notification;

public record NotificationRecipientDto
{
    public Guid UserId { get; init; }

    public string? Email { get; init; }

    public IReadOnlyCollection<string> DeviceTokens { get; init; } = [];
}