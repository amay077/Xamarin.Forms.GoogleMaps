using System;
using System.Collections.Generic;
using Android.Gms.Maps.Model;
using System.Linq;
namespace Xamarin.Forms.GoogleMaps.Android
{
    internal static class PositionExtensions
    {
        public static LatLng ToLatLng(this Position self)
        {
            return new LatLng(self.Latitude, self.Longitude);
        }

        public static IList<LatLng> ToLatLngs(this IList<Position> self)
        {
            return self.Select(ToLatLng).ToList();
        }
    }
}

