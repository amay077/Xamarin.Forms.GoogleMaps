using System;
using Android.Gms.Maps.Model;
using Xamarin.Forms.GoogleMaps.Android.Extensions;

namespace Xamarin.Forms.GoogleMaps.Android
{
    internal static class VisibleRegionExtensions
    {
        public static MapRegion ToRegion(this VisibleRegion visibleRegion)
        {
            return new MapRegion(
                visibleRegion.NearLeft.ToPosition(),
                visibleRegion.NearRight.ToPosition(),
                visibleRegion.FarLeft.ToPosition(),
                visibleRegion.FarRight.ToPosition()
            );
        }        
    }
}
