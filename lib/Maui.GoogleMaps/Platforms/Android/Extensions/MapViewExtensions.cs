using Android.Gms.Maps;
using System.Runtime.CompilerServices;

namespace Maui.GoogleMaps.Android.Extensions
{
    internal static class MapViewExtensions
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Task<GoogleMap> GetGoogleMapAsync(this MapView self)
        {
            var comp = new TaskCompletionSource<GoogleMap>();
            self.GetMapAsync(new OnMapReadyCallback(map =>
            {
                comp.SetResult(map);
            }));

            return comp.Task;
        }
   }

    class OnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
    {
        readonly Action<GoogleMap> handler;

        public OnMapReadyCallback(Action<GoogleMap> handler)
        {
            this.handler = handler;
        }

        void IOnMapReadyCallback.OnMapReady(GoogleMap googleMap)
        {
            handler?.Invoke(googleMap);
        }
    }
}

