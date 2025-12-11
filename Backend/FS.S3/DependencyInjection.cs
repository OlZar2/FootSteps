using FS.Application.Interfaces.Storages;
using FS.Application.Services.ImageLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FS.S3;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IImageStorageService, YandexCloudImageStorageService>();
        
        return services;
    }
}