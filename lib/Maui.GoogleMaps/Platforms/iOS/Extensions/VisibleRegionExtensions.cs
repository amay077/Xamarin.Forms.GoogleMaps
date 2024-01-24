using Google.Maps;

namespace Maui.GoogleMaps.iOS.Extensions;

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