using System;
using Google.Maps;
namespace Xamarin
{
    public static class FormsGoogleMaps
    {
        public static void Init(string apiKey)
        {
            MapServices.ProvideAPIKey(apiKey);
        }
    }
}

