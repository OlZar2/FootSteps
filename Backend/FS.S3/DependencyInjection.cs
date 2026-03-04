using FS.Application.Interfaces.Storages;
using Microsoft.Extensions.DependencyInjection;

namespace FS.S3;

public static class DependencyInjection
{
    public static IServiceCollection AddS3(this IServiceCollection services)
    {
        services
            .AddScoped<IImageStorageService, YandexCloudImageStorageService>();
        
        return services;
    }
}