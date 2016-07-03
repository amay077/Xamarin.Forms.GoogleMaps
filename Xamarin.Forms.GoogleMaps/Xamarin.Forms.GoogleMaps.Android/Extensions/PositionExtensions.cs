using System;
using Android.Gms.Maps.Model;
namespace Xamarin.Forms.GoogleMaps.Android
{
    internal static class PositionExtensions
    {
        public static LatLng ToLatLng(this Position self)
        {
            return new LatLng(self.Latitude, self.Longitude);
        }
    }
}

