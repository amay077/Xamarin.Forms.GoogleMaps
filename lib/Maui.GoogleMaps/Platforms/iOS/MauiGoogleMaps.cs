using Google.Maps;
using Maui.GoogleMaps.Handlers;
using Maui.GoogleMaps.iOS;

namespace Maui;

public static class MauiGoogleMaps
{
    public static bool IsInitialized { get; private set; }

    public static void Init(string apiKey, PlatformConfig config = null)
    {
        MapServices.ProvideApiKey(apiKey);
        GeocoderBackend.Register();
        MapHandler.Config = config ?? new PlatformConfig();
        IsInitialized = true;
    }
}

