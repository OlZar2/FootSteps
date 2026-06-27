using FS.Application.AuthLogic.Configurations;
using FS.Application.NotificationLogic.DTOs;
using FS.Core.Enums.Notifications;
using FS.Email.Templates;
using Microsoft.Extensions.Options;

namespace FS.Email;

public class RazorEmailMessageBodyFactory(
    HtmlEmailRenderer htmlEmailRenderer,
    IOptions<EmailConfirmationOptions> emailConfirmationOptions) : IEmailMessageBodyFactory
{
    public async Task<EmailMessageBody> CreateAsync(EmailNotificationDto notification, CancellationToken ct)
    {
        return notification.Type switch
        {
            NotificationType.EmailConfirmation => await CreateEmailConfirmationAsync(notification, ct),
            _ => new EmailMessageBody(notification.Body)
        };
    }

    private async Task<EmailMessageBody> CreateEmailConfirmationAsync(
        EmailNotificationDto notification,
        CancellationToken ct)
    {
        var confirmationUrl = BuildConfirmationUrl(notification);
        var plainText = $"Для завершения регистрации подтвердите почту: {confirmationUrl}";

        var html = await htmlEmailRenderer.RenderAsync<EmailConfirmationTemplate>(
            new Dictionary<string, object?>
            {
                [nameof(EmailConfirmationTemplate.Model)] = new EmailConfirmationTemplateModel(
                    ConfirmationUrl: confirmationUrl)
            },
            ct);

        return new EmailMessageBody(plainText, html);
    }

    private string BuildConfirmationUrl(EmailNotificationDto notification)
    {
        if (notification.TargetEntityId is null)
            throw new InvalidOperationException("Email confirmation notification must contain target user id.");

        var emailConfirmationToken = notification.EmailConfirmationToken?.Trim();
        if (string.IsNullOrWhiteSpace(emailConfirmationToken))
            throw new InvalidOperationException("Email confirmation notification must contain confirmation token.");

        return emailConfirmationOptions.Value.ConfirmationUrlTemplate
            .Replace("{userId}", Uri.EscapeDataString(notification.TargetEntityId.Value.ToString()))
            .Replace("{token}", Uri.EscapeDataString(emailConfirmationToken));
    }
}
