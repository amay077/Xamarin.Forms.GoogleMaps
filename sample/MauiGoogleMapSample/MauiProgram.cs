using Maui.GoogleMaps.Hosting;

namespace MauiGoogleMapSample
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if ANDROID
            var platformConfig = new Maui.GoogleMaps.Android.PlatformConfig
            {
                BitmapDescriptorFactory = new CachingNativeBitmapDescriptorFactory()
            };

            builder.UseGoogleMaps(platformConfig);
#elif IOS
            var platformConfig = new Maui.GoogleMaps.iOS.PlatformConfig
            {
                ImageFactory = new CachingImageFactory()
            };

            builder.UseGoogleMaps(Variables.GOOGLE_MAPS_IOS_API_KEY, platformConfig);
#endif
            return builder.Build();
        }
    }
}