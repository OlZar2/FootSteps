using Npgsql;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

namespace FS.API.Configurations.Telemetry;

public static class TelemetryConfiguration
{
    public static IServiceCollection AddTelemetryConfiguration(this IServiceCollection services,
        ConfigureHostBuilder host)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource =>
                resource.AddService("fs-api", serviceVersion: "1.0.0"))
            .WithTracing(tracer =>
            {
                tracer
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(o =>
                    {
                        o.RecordException = true;
                    })
                    .AddNpgsql()
                    .AddOtlpExporter(otlp =>
                    {
                        otlp.Endpoint = new Uri("http://localhost:4317");
                        otlp.Protocol = OtlpExportProtocol.Grpc;
                    });
            });
        
        host.UseSerilog((ctx, lc) =>
        {
            lc.ReadFrom.Configuration(ctx.Configuration);
        });
        
        return services;
    }
}