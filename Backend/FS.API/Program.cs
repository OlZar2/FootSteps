using FS.API;
using FS.API.Configurations.ApiServices;
using FS.API.Configurations.Controllers;
using FS.API.Configurations.Cors;
using FS.API.Configurations.Swagger;
using FS.API.Configurations.Telemetry;
using FS.API.Middlewares.Culture;
using FS.API.Middlewares.Errors;
using FS.API.Middlewares.Telemetry;
using FS.API.SecretsLoader;
using FS.Application;
using FS.Email;
using FS.Firebase;
using FS.JWT;
using FS.Persistence;
using FS.Persistence.Context;
using FS.RabbitMq;
using FS.S3;
using FS.SignalR.Hubs;
using Microsoft.EntityFrameworkCore;

FileSecretsLoader.LoadSecretFiles();

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddTelemetryConfiguration(builder.Host, builder.Configuration);

services
    .AddJwtServices()
    .AddServices()
    .AddDatabase(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddOutboxHandling()
    .AddConfiguration(builder.Configuration)
    .AddEmail(builder.Configuration)
    .AddJwtAuth(builder.Configuration)
    .AddFirebase()
    .AddRabbitMq()
    .AddNotificationsHandling()
    .AddApiServices()
    .AddS3();

services.AddSignalR();

services.ConfigureFSSwagger();

services.ConfigureControllers();

services.ConfigureCors();

var app = builder.Build();

app.UseLocalizationMiddleware();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<TracingLoggingMiddleware>();
app.UseCors("DevCors");

app.MapHub<SearchAnnouncementsHub>("/hubs/search-announcements")
    .RequireAuthorization();

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
