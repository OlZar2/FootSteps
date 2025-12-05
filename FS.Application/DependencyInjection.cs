using FS.Application.DomainPolicies.AnimalAnnouncementPolicies;
using FS.Application.DomainPolicies.UserPolicies;
using FS.Application.Services.AnnouncementLogic.Implementations;
using FS.Application.Services.AnnouncementLogic.Interfaces;
using FS.Application.Services.AuthLogic.Implementations;
using FS.Application.Services.AuthLogic.Interfaces;
using FS.Application.Services.FindAnnouncementLogic.Implementations;
using FS.Application.Services.FindAnnouncementLogic.Interfaces;
using FS.Application.Services.ImageLogic.Implementations;
using FS.Application.Services.ImageLogic.Interfaces;
using FS.Application.Services.MissingPetLogic.Implementations;
using FS.Application.Services.MissingPetLogic.Interfaces;
using FS.Application.Services.NotificationLogic.Implementations;
using FS.Application.Services.NotificationLogic.Interfaces;
using FS.Application.Services.SearchLogic.Implementations;
using FS.Application.Services.SearchLogic.Interfaces;
using FS.Application.Services.StreetPetAnnouncementLogic.Implementations;
using FS.Application.Services.StreetPetAnnouncementLogic.Interfaces;
using FS.Application.Services.UserLogic.Implementations;
using FS.Application.Services.UserLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC.Policies;
using FS.Core.UserDomain.UserPolicies;
using Microsoft.Extensions.DependencyInjection;
using FS.Application.Events;
using FS.Application.Services.AnnouncementLogic.Handlers;
using FS.Core.AnimalAnnouncementBC.Events;

namespace FS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IImageStorageService, YandexCloudImageStorageService>()
            .AddScoped<IPasswordHasher, PasswordHasher>()
            .AddScoped<IMissingAnnouncementService, MissingAnnouncementService>()
            .AddScoped<IFindAnnouncementService, FindAnnouncementService>()
            .AddScoped<IStreetPetAnnouncementService, StreetPetAnnouncementService>()
            .AddScoped<IUserService, UserService>()
            .AddScoped<ISearchService, SearchService>()
            .AddScoped<INotificationService, NotificationService>()
            .AddScoped<IAnimalAnnouncementService, AnimalAnnouncementService>();

        services
            .AddScoped<IAnimalAnnouncementDeletionPolicy, DefaultAnimalAnnouncementDeletionPolicy>()
            .AddScoped<IEditUserPolicy, DefaultEditUserPolicy>();
        
        services
            .AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
        
        services.AddScoped<IDomainEventHandler<MissingAnnouncementCreatedDomainEvent>, MissingAnnouncementCreatedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<StreetPetAnnouncementEmbeddingCalculatedDomainEvent>, StreetPetAnnouncementEmbeddingCalculatedDomainEventHandler>();
        services.AddScoped<IDomainEventHandler<AnnouncementCreatedDomainEvent>, AnnouncementCreatedDomainEventHandler>();

        return services;
    }
}