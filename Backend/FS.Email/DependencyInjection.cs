using FS.Application.Interfaces.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FS.Email;

public static class DependencyInjection
{
    public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpEmailOptions>(
            configuration.GetSection(nameof(SmtpEmailOptions)));

        services.AddSingleton<IEmailNotificationSender, SmtpEmailNotificationSender>();

        return services;
    }
}
