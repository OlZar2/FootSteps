using FS.Application.Interfaces.Notifications;
using FS.Application.NotificationLogic.DTOs;
using FS.Application.NotificationLogic.Exceptions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FS.Email;

public class SmtpEmailNotificationSender(
    IOptions<SmtpEmailOptions> options,
    ILogger<SmtpEmailNotificationSender> logger,
    IEmailMessageBodyFactory emailMessageBodyFactory) : IEmailNotificationSender
{
    public async Task SendAsync(EmailNotificationDto notification, CancellationToken ct)
    {
        try
        {
            var smtpOptions = options.Value;
            if (string.IsNullOrWhiteSpace(smtpOptions.Host))
                throw new InvalidOperationException("SMTP host is not configured.");

            if (string.IsNullOrWhiteSpace(smtpOptions.FromEmail))
                throw new InvalidOperationException("SMTP from email is not configured.");

            var shouldAuthenticate =
                !string.IsNullOrWhiteSpace(smtpOptions.UserName) ||
                !string.IsNullOrWhiteSpace(smtpOptions.Password);

            if (shouldAuthenticate &&
                (string.IsNullOrWhiteSpace(smtpOptions.UserName) ||
                 string.IsNullOrWhiteSpace(smtpOptions.Password)))
            {
                throw new InvalidOperationException("Both SMTP username and password must be configured.");
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(smtpOptions.FromName, smtpOptions.FromEmail));
            message.To.Add(MailboxAddress.Parse(notification.RecipientEmail));
            message.Subject = notification.Subject;
            message.Body = (await emailMessageBodyFactory.CreateAsync(notification, ct)).ToMimeEntity();

            using var client = new SmtpClient();
            var socketOptions = smtpOptions.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTlsWhenAvailable;

            await client.ConnectAsync(smtpOptions.Host, smtpOptions.Port, socketOptions, ct);

            if (shouldAuthenticate)
            {
                await client.AuthenticateAsync(smtpOptions.UserName, smtpOptions.Password, ct);
            }

            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error sending email notification to {RecipientEmail}", notification.RecipientEmail);
            throw new NotificationDeliveryException();
        }
    }
}
