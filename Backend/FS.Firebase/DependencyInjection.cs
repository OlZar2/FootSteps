using FirebaseAdmin;
using FS.Application.Interfaces.Notifications;
using FS.Firebase.Senders;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.DependencyInjection;

namespace FS.Firebase;

public static class DependencyInjection
{
    public static IServiceCollection AddFirebase(this IServiceCollection services)
    {
        var firebaseApp = FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.GetApplicationDefault()
        });
        
        services.AddSingleton(firebaseApp);
        services.AddSingleton<IPushNotificationSender, FirebasePushNotificationSender>();
        
        return services;
    }
}