#if WINDOWS_UWP
using Xamarin.Forms.GoogleMaps.UWP;

#else
using Xamarin.Forms.Maps.WinRT;

#endif

namespace Xamarin
{
    public static class FormsGoogleMaps
    {
        static bool s_isInitialized;

        internal static string AuthenticationToken { get; set; }

        public static void Init()
        {
            if (s_isInitialized)
                return;
            GeocoderBackend.Register();
            s_isInitialized = true;
        }

        public static void Init(string authenticationToken)
        {
            AuthenticationToken = authenticationToken;

            Init();
        }
    }
}