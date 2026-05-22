using System.Text.Json;
using FluentValidation;
using FS.API.Controllers.Auth.RequestModels.Validators;

namespace FS.API.Configurations.Controllers;

public static class ControllersConfiguration
{
    public static IServiceCollection ConfigureControllers(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
            })
            .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

        services.AddValidatorsFromAssemblyContaining<RegisterRMValidator>();
        
        return services;
    }
}