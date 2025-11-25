using FS.Application.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Core.Stores;
using FS.Persistence.Outbox;
using FS.Persistence.Outbox.Handlers.Implementations;
using FS.Persistence.Outbox.Handlers.Interfaces;
using FS.Persistence.QueryServices;
using FS.Persistence.Repositories;
using FS.Persistence.Services;
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
            .AddScoped<ITransactionService, TransactionService>()
            .AddScoped<IOutboxRepository, OutboxRepository>()
            .AddScoped<ISearchRequestRepository, EFSearchRequestRepository>();

        services
            .AddScoped<IMissingAnnouncementQueryService, EFMissingAnnouncementQueryService>()
            .AddScoped<IFindAnnouncementQueryService, EFFindAnnouncementQueryService>()
            .AddScoped<IStreetPetAnnouncementQueryService, EFStreetPetAnnouncementQueryService>()
            .AddScoped<IUserQueryService, EFUserQueryService>()
            .AddScoped<ISearchQueryService, EFSearchQueryService>();

        return services;
    }

    public static IServiceCollection AddOutboxHandling(this IServiceCollection services)
    {
        services
            .AddHostedService<OutboxDispatcher>();
        
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
}