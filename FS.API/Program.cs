using System.Text.Json;
using FluentValidation;
using FS.API;
using FS.API.Middlewares;
using FS.API.RequestsModels.Auth.Validators;
using FS.Application;
using FS.JWT;
using FS.Persistence;
using FS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

builder.Configuration.AddUserSecrets<Program>(optional: true);

services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
        x =>
        {
            x.UseNetTopologySuite();
            x.MigrationsAssembly("FS.Migrations");
        }));

services
    .AddJwt()
    .AddServices()
    .AddRepositories()
    .AddConfiguration(builder.Configuration);

services.AddEndpointsApiExplorer();
services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Footsteps API",
        Version = "v1"
    });

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

app.MapControllers();


app.UseSwagger();
app.UseSwaggerUI();
app.UseMiddleware<ErrorHandlingMiddleware>();

app.Run();