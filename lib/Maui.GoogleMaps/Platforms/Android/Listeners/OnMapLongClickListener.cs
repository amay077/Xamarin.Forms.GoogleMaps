using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Maui.GoogleMaps.Android.Extensions;
using Maui.GoogleMaps.Handlers;

namespace Maui.GoogleMaps.Platforms.Android.Listeners;

internal class OnMapLongClickListener : Java.Lang.Object, GoogleMap.IOnMapLongClickListener
{
    public MapHandler MapHandler { get; set; }

    public void OnMapLongClick(LatLng point)
    {
        MapHandler.VirtualView.SendMapLongClicked(point.ToPosition());
    }
}