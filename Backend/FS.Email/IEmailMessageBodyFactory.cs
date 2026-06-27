using FS.Application.NotificationLogic.DTOs;

namespace FS.Email;

public interface IEmailMessageBodyFactory
{
    Task<EmailMessageBody> CreateAsync(EmailNotificationDto notification, CancellationToken ct);
}
