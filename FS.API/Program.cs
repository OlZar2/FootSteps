using System.Globalization;
using System.Reflection;
using System.Text.Json;
using FluentValidation;
using FS.API;
using FS.API.Middlewares;
using FS.API.RequestsModels.Auth.Validators;
using FS.API.Services.ClaimLogic.Implementations;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.API.Services.GeoLogic.Implementations;
using FS.API.Services.GeoLogic.Interfaces;
using FS.API.Services.ImageLogic;
using FS.Application;
using FS.JWT;
using FS.Persistence;
using FS.Persistence.Context;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Configuration.AddUserSecrets<Program>(optional: true);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables()
    .AddKeyPerFile(directoryPath: "/run/secrets", optional: true);

services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        x =>
        {
            x.UseNetTopologySuite();
            x.MigrationsAssembly("FS.Migrations");
        }));

services
    .AddJwtServices()
    .AddServices()
    .AddRepositories()
    .AddConfiguration(builder.Configuration)
    .AddJwtAuth(builder.Configuration);

services
    .AddTransient<IClaimService, ClaimService>()
    .AddTransient<ImageService>()
    .AddHttpClient<IGeocoder, YandexGeocoder>();

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

services
    .AddControllers(options =>
    {
        options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    })
    .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

services.AddValidatorsFromAssemblyContaining<RegisterRMValidator>();
var app = builder.Build();

var enUS = new CultureInfo("en-US");

CultureInfo.DefaultThreadCurrentCulture = enUS;
CultureInfo.DefaultThreadCurrentUICulture = enUS;

var locOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(enUS),
    SupportedCultures = new[] { enUS },
    SupportedUICultures = new[] { enUS }
};

locOptions.RequestCultureProviders.Clear();

app.UseRequestLocalization(locOptions);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.Run();