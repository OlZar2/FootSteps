using System.Reflection;
using Microsoft.OpenApi.Models;

namespace FS.API.Configurations.Swagger;

public static class FSSwaggerConfiguration
{
    public static IServiceCollection ConfigureFSSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(config =>
        {
            config.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Footsteps API",
                Version = "v1"
            });
    
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            config.IncludeXmlComments(xmlPath);
            config.UseInlineDefinitionsForEnums();

            config.EnableAnnotations();
        });
        
        return services;
    }
}