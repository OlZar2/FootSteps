using FS.Application.Interfaces;
using FS.Application.Interfaces.QueryServices;
using FS.Core.Stores;
using FS.Persistence.Outbox;
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
            .AddScoped<IOutboxRepository, OutboxRepository>();

        services
            .AddScoped<IMissingAnnouncementQueryService, EFMissingAnnouncementQueryService>()
            .AddScoped<IFindAnnouncementQueryService, EFFindAnnouncementQueryService>()
            .AddScoped<IStreetPetAnnouncementQueryService, EFStreetPetAnnouncementQueryService>()
            .AddScoped<IUserQueryService, EFUserQueryService>();
        
        services.AddHostedService<OutboxDispatcher>();

        return services;
    }
}