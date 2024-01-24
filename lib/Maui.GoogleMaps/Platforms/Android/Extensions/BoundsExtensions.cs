using Android.Gms.Maps.Model;

namespace Maui.GoogleMaps.Android.Extensions;

internal static class BoundsExtensions
{
    public static LatLngBounds ToLatLngBounds(this Bounds self)
    {
        return new LatLngBounds(self.SouthWest.ToLatLng(), self.NorthEast.ToLatLng());
    }
}