using Google.Maps;

namespace Maui.GoogleMaps.iOS.Extensions;

internal static class EnumerableExtensions
{
    public static MutablePath ToMutablePath(this IEnumerable<Position> enumerable)
    {
        var path = new MutablePath();
        foreach (var position in enumerable)
        {
            path.AddLatLon(position.Latitude, position.Longitude);
        }

        return path;
    }
}