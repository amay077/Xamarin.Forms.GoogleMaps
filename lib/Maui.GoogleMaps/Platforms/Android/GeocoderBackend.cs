using Android.Content;
using AGeocoder = Android.Locations.Geocoder;

namespace Maui.GoogleMaps.Android;

internal class GeocoderBackend
{
    private static Context _context;

    public static void Register(Context context)
    {
        _context = context;
        Geocoder.GetPositionsForAddressAsyncFunc = GetPositionsForAddressAsync;
        Geocoder.GetAddressesForPositionFuncAsync = GetAddressesForPositionAsync;
    }

    public static async Task<IEnumerable<Position>> GetPositionsForAddressAsync(string address)
    {
        var geocoder = new AGeocoder(_context);
        var addresses = await geocoder.GetFromLocationNameAsync(address, 5);
        return addresses.Select(p => new Position(p.Latitude, p.Longitude));
    }

    public static async Task<IEnumerable<string>> GetAddressesForPositionAsync(Position position)
    {
        var geocoder = new AGeocoder(_context);
        var addresses = await geocoder.GetFromLocationAsync(position.Latitude, position.Longitude, 5);
        return addresses.Select(p =>
        {
            var lines = Enumerable.Range(0, p.MaxAddressLineIndex + 1).Select(p.GetAddressLine);
            return string.Join("\n", lines);
        });
    }
}