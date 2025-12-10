using FS.API.Services.ClaimLogic.Implementations;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.API.Services.GeoLogic.Implementations;
using FS.API.Services.GeoLogic.Interfaces;

namespace FS.API.Configurations.ApiServices;

public static class ApiServicesConfiguration
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services
            .AddTransient<IClaimService, ClaimService>()
            .AddHttpClient<IGeocoder, YandexGeocoder>();
        
        return services;
    }
}