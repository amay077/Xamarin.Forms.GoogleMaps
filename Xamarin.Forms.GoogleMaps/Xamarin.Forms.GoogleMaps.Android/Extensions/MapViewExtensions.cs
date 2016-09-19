using Android.Gms.Maps;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System;

namespace Xamarin.Forms.GoogleMaps.Android.Extensions
{
    internal static class MapViewExtensions
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static Task<GoogleMap> GetGoogleMapAsync(this MapView self)
        {
            try
            {
                var googleMap = GetMap(self);

                if (googleMap != null)
                {
                    return Task.FromResult(googleMap);
                }
            }
            catch (MissingMethodException)
            {
            }

            var comp = new TaskCompletionSource<GoogleMap>();
            self.GetMapAsync(new OnMapReadyCallback(map => 
            {
                comp.SetResult(map);
            }));

            return comp.Task;
        }

        static GoogleMap GetMap(MapView mapView)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return mapView.Map;
#pragma warning restore CS0618 // Type or member is obsolete
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

