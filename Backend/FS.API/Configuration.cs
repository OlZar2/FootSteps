using FS.API.Services.GeoLogic.Options;
using FS.Application.AnnouncementLogic.Configurations;
using FS.Application.AuthLogic.Configurations;
using FS.Application.ImageLogic.Configurations;
using FS.Application.Shared.Configurations;
using FS.JWT;
using FS.RabbitMq.Options;

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

        services.Configure<YandexApiOptions>(
            configuration.GetSection(nameof(YandexApiOptions)));

        services.Configure<RabbitMqOptions>(
            configuration.GetSection(nameof(RabbitMqOptions)));

        services.Configure<ImageEmbeddingRabbitOptions>(
            configuration.GetSection(nameof(ImageEmbeddingRabbitOptions)));

        services.Configure<ImagesOptions>(
            configuration.GetSection(nameof(ImagesOptions)));

        services.Configure<EmailConfirmationOptions>(
            configuration.GetSection(nameof(EmailConfirmationOptions)));

        services.Configure<AdminAnimalAnnouncementListOptions>(
            configuration.GetSection(nameof(AdminAnimalAnnouncementListOptions)));

        return services;
    }
}
