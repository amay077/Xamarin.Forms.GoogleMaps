using System;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.Extensions.iOS;
namespace Xamarin.Forms.GoogleMaps.iOS.Extensions
{
    internal static class BoundsExtensions
    {
        public static CoordinateBounds ToCoordinateBounds(this Bounds self)
        {
            return new CoordinateBounds(self.SouthWest.ToCoord(), self.NorthEast.ToCoord());
        }
    }
}

