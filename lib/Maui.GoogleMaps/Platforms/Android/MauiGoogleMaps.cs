using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.OS;
using Maui.GoogleMaps.Android;
using Maui.GoogleMaps.Handlers;

namespace Maui;

public static class MauiGoogleMaps
{
    public static bool IsInitialized { get; private set; }

    public static Context Context { get; private set; }

    public static void Init(Activity activity, Bundle bundle, PlatformConfig config = null)
    {
        if (IsInitialized)
        {
            return;
        }

        Context = activity;

        MapHandler.Bundle = bundle;
        MapHandler.Config = config ?? new PlatformConfig();

#pragma warning disable 618
        if (GooglePlayServicesUtil.IsGooglePlayServicesAvailable(Context) == ConnectionResult.Success)
#pragma warning restore 618
        {
            try
            {
                MapsInitializer.Initialize(Context, MapsInitializer.Renderer.Latest, null);
                IsInitialized = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Google Play Services Not Found");
                Console.WriteLine("Exception: {0}", e);
            }
        }

        GeocoderBackend.Register(Context);
    }
}