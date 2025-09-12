using FS.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FS.JWT;

public static class DependencyInjection
{
    public static IServiceCollection AddJwtServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }
}