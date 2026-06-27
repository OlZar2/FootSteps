using System.Text.RegularExpressions;
using FS.Application.NotificationLogic.DTOs;
using FS.Core.Enums.Notifications;
using FS.Email.Templates;

namespace FS.Email;

public class RazorEmailMessageBodyFactory(HtmlEmailRenderer htmlEmailRenderer) : IEmailMessageBodyFactory
{
    private static readonly Regex AbsoluteUrlRegex = new(
        @"https?://[^\s<>""]+",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

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
        var confirmationUrl = ExtractConfirmationUrl(notification.Body);
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

    private static string ExtractConfirmationUrl(string body)
    {
        var trimmedBody = body.Trim();
        if (Uri.TryCreate(trimmedBody, UriKind.Absolute, out _))
            return trimmedBody;

        var match = AbsoluteUrlRegex.Match(body);
        if (match.Success)
            return match.Value.TrimEnd('.', ',', ';', ')');

        return trimmedBody;
    }
}
