using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Maui.GoogleMaps.Android.Extensions;
using Maui.GoogleMaps.Handlers;

namespace Maui.GoogleMaps.Platforms.Android.Listeners;

internal class OnMapClickListener : Java.Lang.Object, GoogleMap.IOnMapClickListener
{
    public MapHandler MapHandler { get; set; }

    public void OnMapClick(LatLng point)
    {
        MapHandler.VirtualView.SendMapClicked(point.ToPosition());
    }
}