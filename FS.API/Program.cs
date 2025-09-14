using System.Reflection;
using System.Text.Json;
using FluentValidation;
using FS.API;
using FS.API.Middlewares;
using FS.API.RequestsModels.Auth.Validators;
using FS.API.Services.ClaimLogic.Implementations;
using FS.API.Services.ClaimLogic.Interfaces;
using FS.Application;
using FS.JWT;
using FS.Persistence;
using FS.Persistence.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Configuration.AddUserSecrets<Program>(optional: true);

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
    .AddTransient<IClaimService, ClaimService>();

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
services.AddFluentValidationAutoValidation(c =>
{
    c.DisableBuiltInModelValidation = true;
});
var app = builder.Build();

app.MapControllers();


app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.Run();