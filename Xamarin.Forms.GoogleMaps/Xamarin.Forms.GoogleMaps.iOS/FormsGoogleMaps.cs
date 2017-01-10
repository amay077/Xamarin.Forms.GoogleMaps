using System;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.iOS;

namespace Xamarin
{
    public static class FormsGoogleMaps
    {
        public static bool IsInitialized { get; private set; }

        public static void Init(string apiKey)
        {
            MapServices.ProvideAPIKey(apiKey);
            GeocoderBackend.Register();
            IsInitialized = true;
        }
    }
}

