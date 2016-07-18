using System;
using Android.Gms.Maps.Model;
namespace Xamarin.Forms.GoogleMaps.Android.Extensions
{
    internal static class LatLngExtensions
    {
        public static Position ToPosition(this LatLng self)
        {
            return new Position(self.Longitude, self.Longitude);
        }
    }
}

