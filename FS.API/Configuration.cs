using FS.Application.Services.ImageLogic.Configurations;

namespace FS.API;

public static class Configuration
{
    public static IServiceCollection AddConfiguration(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3StorageConfiguration>(
            configuration.GetSection(nameof(S3StorageConfiguration)));
        
        return services;
    }
}