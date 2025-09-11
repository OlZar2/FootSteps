using FS.Application.Interfaces;
using FS.Core.Stores;
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
            .AddScoped<ITransactionService, TransactionService>();

        return services;
    }
}