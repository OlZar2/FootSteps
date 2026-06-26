using FS.Application.Interfaces.QueryServices;
using FS.Application.Interfaces.Transaction;
using FS.Core.AnimalAnnouncementBC.Stores;
using FS.Core.NotificationDomain.Stores;
using FS.Core.OutboxDomain.Stores;
using FS.Core.ReadDomain.Stores;
using FS.Core.SearchDomain.Stores;
using FS.Core.UserDomain.Stores;
using FS.Persistence.Context;
using FS.Persistence.Outbox.Notifications;
using FS.Persistence.Outbox.Notifications.Handlers;
using FS.Persistence.Outbox.Shared.Interfaces;
using FS.Persistence.Outbox.SharedOutbox;
using FS.Persistence.Outbox.SharedOutbox.Handlers;
using FS.Persistence.QueryServices;
using FS.Persistence.Repositories;
using FS.Persistence.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FS.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
    {
        services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<IMissingAnnouncementRepository, MissingAnnouncementRepository>()
            .AddScoped<IFindAnnouncementRepository, FindAnnouncementRepository>()
            .AddScoped<IStreetPetAnnouncementRepository, StreetPetAnnouncementRepository>()
            .AddScoped<IOutboxRepository, EFOutboxRepository>()
            .AddScoped<ISearchRequestRepository, EFSearchRequestRepository>()
            .AddScoped<INotificationRepository, EFNotificationRepository>()
            .AddScoped<ISimilarAnnouncementRepository, EFSimilarAnnouncementRepository>()
            .AddScoped<IAnimalAnnouncementRepository, EFAnimalAnnouncementRepository>()
            .AddScoped<IImageRepository, EFImageRepository>();

        services
            .AddScoped<IMissingAnnouncementQueryService, EFMissingAnnouncementQueryService>()
            .AddScoped<IFindAnnouncementQueryService, EFFindAnnouncementQueryService>()
            .AddScoped<IStreetPetAnnouncementQueryService, EFStreetPetAnnouncementQueryService>()
            .AddScoped<IUserQueryService, EFUserQueryService>()
            .AddScoped<ISearchQueryService, EFSearchQueryService>()
            .AddScoped<IFoundReportsQueryService, EFFoundReportsQueryService>()
            .AddScoped<IUserDeviceQueryService, EFUserDevicesQueryService>()
            .AddScoped<INotificationDeliveryQueryService, EFNotificationDeliveryQueryService>()
            .AddScoped<IAnimalAnnouncementQueryService, EFAnimalAnnouncementQueryService>()
            .AddScoped<ISpottedLocationsQueryService, EFSpottedLocationsQueryService>()
            .AddScoped<IImageQueryService, EFAnimalAnnouncementImageQueryService>();
            
        
        services.AddScoped<ITransactionFactory, EfCoreTransactionFactory>();
        
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString, 
                x =>
                {
                    x.UseVector();
                    x.UseNetTopologySuite();
                    x.MigrationsAssembly("FS.Migrations");
                }));

        return services;
    }

    public static IServiceCollection AddOutboxHandling(this IServiceCollection services)
    {
        services
            .AddHostedService<OutboxWorker>();
        
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
            var fifthDecorator = ActivatorUtilities
                .CreateInstance<FindRecipientsForMissingAnnouncementNotificationOutboxHandler>(sp, fourthDecorator);
            return fifthDecorator;
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