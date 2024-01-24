using Android.Gms.Maps;
using Maui.GoogleMaps.Handlers;

namespace Maui.GoogleMaps.Platforms.Android.Listeners;

internal class OnMyLocationButtonClickListener : Java.Lang.Object, GoogleMap.IOnMyLocationButtonClickListener
{
    public MapHandler MapHandler { get; set; }

    public bool OnMyLocationButtonClick()
    {
        return MapHandler.VirtualView.SendMyLocationClicked();
    }
}