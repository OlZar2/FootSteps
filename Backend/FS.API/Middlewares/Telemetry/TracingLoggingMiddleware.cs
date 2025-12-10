using System.Diagnostics;
using Serilog.Context;

namespace FS.API.Middlewares.Telemetry;

public class TracingLoggingMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var activity = Activity.Current;

        if (activity != null)
        {
            using (LogContext.PushProperty("TraceId", activity.TraceId.ToString()))
            using (LogContext.PushProperty("SpanId", activity.SpanId.ToString()))
            {
                await next(context);
            }
        }
        else
        {
            await next(context);
        }
    }
}