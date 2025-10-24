using FS.Application.Interfaces.Events;
using FS.RabbitMq.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FS.RabbitMq;

public static class DependencyInjection
{
    public static IServiceCollection AddRabbitMq(this IServiceCollection services)
    {
        services.AddSingleton<IMessageBus, RabbitMqPublisher>();
        services.AddHostedService<EmbedResponseConsumer>();
        
        return services;
    }
}