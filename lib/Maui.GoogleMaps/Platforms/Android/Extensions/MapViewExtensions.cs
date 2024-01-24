using Android.Gms.Maps;
using Maui.GoogleMaps.Platforms.Android.Callbacks;
using System.Runtime.CompilerServices;

namespace Maui.GoogleMaps.Android.Extensions;

internal static class MapViewExtensions
{
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static Task<GoogleMap> GetGoogleMapAsync(this MapView self)
    {
        var comp = new TaskCompletionSource<GoogleMap>();
        self.GetMapAsync(new OnMapReadyCallback(map =>
        {
            comp.SetResult(map);
        }));

        return comp.Task;
    }
}