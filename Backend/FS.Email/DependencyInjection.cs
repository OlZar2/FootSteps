using FS.Application.Interfaces.Notifications;
using FS.Email.Templates;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FS.Email;

public static class DependencyInjection
{
    public static IServiceCollection AddEmail(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SmtpEmailOptions>(
            configuration.GetSection(nameof(SmtpEmailOptions)));

        services.AddScoped<HtmlRenderer>();
        services.AddScoped<HtmlEmailRenderer>();
        services.AddScoped<IEmailMessageBodyFactory, RazorEmailMessageBodyFactory>();
        services.AddScoped<IEmailNotificationSender, SmtpEmailNotificationSender>();

        return services;
    }
}
