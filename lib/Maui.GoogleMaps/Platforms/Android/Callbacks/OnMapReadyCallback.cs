using Android.Gms.Maps;

namespace Maui.GoogleMaps.Platforms.Android.Callbacks;

internal class OnMapReadyCallback : Java.Lang.Object, IOnMapReadyCallback
{
    private readonly Action<GoogleMap> _handler;

    public OnMapReadyCallback(Action<GoogleMap> handler)
    {
        _handler = handler;
    }

    void IOnMapReadyCallback.OnMapReady(GoogleMap googleMap)
    {
        _handler?.Invoke(googleMap);
    }
}