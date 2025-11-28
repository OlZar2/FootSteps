using Microsoft.Extensions.DependencyInjection;

namespace FS.Notifications;

public static class DependencyInjection
{
    public static IServiceCollection AddDomainEventPublisher(this IServiceCollection services)
    {
        services.AddScoped<IDomainEventPublisher, NotificationsDomainEventPublisher>();
        
        return services;
    }
}