using Maui.GoogleMaps.Handlers;
using Microsoft.Maui.LifecycleEvents;

namespace Maui.GoogleMaps.Hosting
{
    public static class AppHostBuilderExtensions
    {
        public static MauiAppBuilder UseGoogleMaps(this MauiAppBuilder appBuilder,
#if ANDROID
            Android.PlatformConfig config = null)
#elif IOS
            string iosApiKey, iOS.PlatformConfig config = null)
#endif
        {
            appBuilder.ConfigureMauiHandlers(handlers => handlers.AddTransient(typeof(Map), h => new MapHandler()))
            .ConfigureLifecycleEvents(events =>
            {
#if ANDROID
                events.AddAndroid(android => android
                .OnCreate((activity, bundle) => MauiGoogleMaps.Init(activity, bundle, config)));
#elif IOS
                events.AddiOS(ios => ios
                .WillFinishLaunching((app, options) =>
                { 
                    MauiGoogleMaps.Init(iosApiKey, config);
                    return true;
                }));
#endif
            });
            return appBuilder;
        }
    }
}
