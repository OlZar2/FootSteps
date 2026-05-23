using FS.Application.AnnouncementLogic.Handlers;
using FS.Application.AnnouncementLogic.Implementations;
using FS.Application.AnnouncementLogic.Interfaces;
using FS.Application.AnnouncementLogic.Policies;
using FS.Application.AuthLogic.Implementations;
using FS.Application.AuthLogic.Interfaces;
using FS.Application.EventLogic.Implementations;
using FS.Application.EventLogic.Interfaces;
using FS.Core.AnimalAnnouncementBC.Policies;
using FS.Core.UserDomain.UserPolicies;
using Microsoft.Extensions.DependencyInjection;
using FS.Application.FindAnnouncementLogic.Implementations;
using FS.Application.FindAnnouncementLogic.Interfaces;
using FS.Application.MissingPetLogic.EventHandlers;
using FS.Application.MissingPetLogic.Implementations;
using FS.Application.MissingPetLogic.Interfaces;
using FS.Application.NotificationLogic.Implementations;
using FS.Application.NotificationLogic.Interfaces;
using FS.Application.SearchLogic.Implementations;
using FS.Application.SearchLogic.Interfaces;
using FS.Application.StreetPetAnnouncementLogic.Implementations;
using FS.Application.StreetPetAnnouncementLogic.Interfaces;
using FS.Application.UserLogic.Implementations;
using FS.Application.UserLogic.Interfaces;
using FS.Application.UserLogic.Policies;
using FS.Core.AnimalAnnouncementBC.Events;
using NetTopologySuite;

namespace FS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddScoped<IAuthService, AuthService>()
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
        
        services.AddSingleton(provider =>
            NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326));
        
        services.AddScoped<IDomainEventHandler<MissingAnnouncementCreatedDomainEvent>, MissingAnnouncementCreatedDomainEventHandler>()
            .AddScoped<IDomainEventHandler<StreetPetAnnouncementEmbeddingCalculatedDomainEvent>, StreetPetAnnouncementEmbeddingCalculatedDomainEventHandler>()
            .AddScoped<IDomainEventHandler<AnnouncementCreatedDomainEvent>, AnnouncementCreatedDomainEventHandler>()
            .AddScoped<IDomainEventHandler<ReportFoundDomainEvent>, ReportFoundDomainEventHandler>()
            .AddScoped<IDomainEventHandler<ReportSpottedDomainEvent>, ReportSpottedDomainEventHandler>();

        return services;
    }
}