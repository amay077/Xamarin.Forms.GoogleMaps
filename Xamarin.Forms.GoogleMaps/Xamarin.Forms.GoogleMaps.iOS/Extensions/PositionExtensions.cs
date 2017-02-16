using System;
using CoreLocation;

namespace Xamarin.Forms.GoogleMaps.iOS.Extensions
{
    internal static class PositionExtensions
    {
        public static CLLocationCoordinate2D ToCoord(this Position self)
        {
            return new CLLocationCoordinate2D(self.Latitude, self.Longitude);
        }
    }
}

