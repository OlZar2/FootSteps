using FS.Application.DomainPolicies.AnimalAnnouncementPolicies;
using FS.Application.DomainPolicies.UserPolicies;
using FS.Application.Services.AuthLogic.Implementations;
using FS.Application.Services.AuthLogic.Interfaces;
using FS.Application.Services.FindAnnouncementLogic.Implementations;
using FS.Application.Services.FindAnnouncementLogic.Interfaces;
using FS.Application.Services.ImageLogic.Implementations;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Application.Services.MissingPetLogic.Implementations;
using FS.Application.Services.MissingPetLogic.Interfaces;
using FS.Application.Services.UserLogic.Implementations;
using FS.Application.Services.UserLogic.Interfaces;
using FS.Core.Policies.AnnouncementPolicies;
using FS.Core.Policies.UserPolicies;
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
            .AddScoped<IPasswordHasher, PasswordHasher>()
            .AddScoped<IMissingAnnouncementService, MissingAnnouncementService>()
            .AddScoped<IFindAnnouncementService, FindAnnouncementService>()
            .AddScoped<IUserService, UserService>();

        services
            .AddScoped<IEmailUniqueService, EmailUniqueService>();

        services
            .AddScoped<IAnimalAnnouncementDeletionPolicy, DefaultAnimalAnnouncementDeletionPolicy>()
            .AddScoped<IEditUserPolicy, DefaultEditUserPolicy>();

        return services;
    }
}