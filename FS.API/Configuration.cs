using FS.Application.Services.ImageLogic.Configurations;
using FS.JWT;

namespace FS.API;

public static class Configuration
{
    public static IServiceCollection AddConfiguration(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<S3StorageConfiguration>(
            configuration.GetSection(nameof(S3StorageConfiguration)));
        
        services.Configure<JwtOptions>(
            configuration.GetSection(nameof(JwtOptions)));
        
        return services;
    }
}