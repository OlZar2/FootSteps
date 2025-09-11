using FS.Application.Services.AuthLogic.Implementations;
using FS.Application.Services.AuthLogic.Interfaces;
using FS.Application.Services.ImageLogic.Implementations;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Core.Services;
using HW.Application.Services.AuthLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IImageService, YandexCloudImageService>()
            .AddScoped<IPasswordHasher, PasswordHasher>();

        services
            .AddScoped<IEmailUniqueService, EmailUniqueService>();

        return services;
    }
}