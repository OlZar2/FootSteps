using FS.Application.Services.GeoLogic.Implementations;
using FS.Application.Services.GeoLogic.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FS.Overpass;

public static class DependencyInjection
{
    public static IServiceCollection AddOverpass(this IServiceCollection services, HttpClient httpClient)
    {
        services.AddScoped<IGeoService, OverpassGeoService>();
        
        return services;
    }
}