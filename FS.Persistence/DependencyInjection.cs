using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Transaction;
using FS.Core.Stores;
using FS.Persistence.Outbox.Embeddings;
using FS.Persistence.Outbox.Embeddings.Handlers;
using FS.Persistence.Outbox.Notifications;
using FS.Persistence.Outbox.Notifications.Handlers;
using FS.Persistence.Outbox.Shared.Interfaces;
using FS.Persistence.QueryServices;
using FS.Persistence.Repositories;
using FS.Persistence.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace FS.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IImageRepository, ImageRepository>()
            .AddScoped<IMissingAnnouncementRepository, MissingAnnouncementRepository>()
            .AddScoped<IFindAnnouncementRepository, FindAnnouncementRepository>()
            .AddScoped<IStreetPetAnnouncementRepository, StreetPetAnnouncementRepository>()
            .AddScoped<IOutboxRepository, OutboxRepository>()
            .AddScoped<ISearchRequestRepository, EFSearchRequestRepository>()
            .AddScoped<INotificationRepository, EFNotificationRepository>()
            .AddScoped<INotificationDeliveryRepository, EFNotificationDeliveryRepository>()
            .AddScoped<IUserDeviceRepository, EFUserDeviceRepository>();

        services
            .AddScoped<IMissingAnnouncementQueryService, EFMissingAnnouncementQueryService>()
            .AddScoped<IFindAnnouncementQueryService, EFFindAnnouncementQueryService>()
            .AddScoped<IStreetPetAnnouncementQueryService, EFStreetPetAnnouncementQueryService>()
            .AddScoped<IUserQueryService, EFUserQueryService>()
            .AddScoped<ISearchQueryService, EFSearchQueryService>();
        
        services.AddScoped<ITransactionFactory, EfCoreTransactionFactory>();

        return services;
    }

    public static IServiceCollection AddOutboxHandling(this IServiceCollection services)
    {
        services
            .AddHostedService<EmbeddingsOutboxWorker>();
        
        services.AddScoped<UnknownOutboxHandler>();
        
        services.AddScoped<IOutboxHandler>(sp =>
        {
            var firstDecorator = ActivatorUtilities.CreateInstance<EmbeddingRequestOutboxHandler>(
                    sp, sp.GetRequiredService<UnknownOutboxHandler>());
            var secondDecorator =
                ActivatorUtilities.CreateInstance<ImageFindSimilarMissingOutboxHandler>(sp, firstDecorator);
            var thirdDecorator = ActivatorUtilities
                .CreateInstance<ImageSearchMatchOutboxHandler>(sp, secondDecorator);
            var fourthDecorator = ActivatorUtilities
                .CreateInstance<ImageSearchRequestOutboxHandler>(sp, thirdDecorator);
            return fourthDecorator;
        });
        
        return services;
    }
    
    public static IServiceCollection AddNotificationsHandling(this IServiceCollection services)
    {
        services
            .AddHostedService<NotificationsOutboxWorker>();
        
        services.AddScoped<INotificationPipelineHandler, PushNotificationPipelineHandler>();
        
        return services;
    }
}