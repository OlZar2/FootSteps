using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace FS.API.Middlewares.Culture;

public static class LocalizationMiddleware
{
    public static IApplicationBuilder UseLocalizationMiddleware(this IApplicationBuilder app)
    {
        var enUS = new CultureInfo("en-US");

        CultureInfo.DefaultThreadCurrentCulture = enUS;
        CultureInfo.DefaultThreadCurrentUICulture = enUS;

        var locOptions = new RequestLocalizationOptions
        {
            DefaultRequestCulture = new RequestCulture(enUS),
            SupportedCultures = [enUS],
            SupportedUICultures = [enUS]
        };

        locOptions.RequestCultureProviders.Clear();
        
        app.UseRequestLocalization(locOptions);
        
        return app;
    }
}