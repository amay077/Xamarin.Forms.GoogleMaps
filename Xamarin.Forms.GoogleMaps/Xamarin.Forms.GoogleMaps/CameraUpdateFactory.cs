using System;
namespace Xamarin.Forms.GoogleMaps
{
    public static class CameraUpdateFactory
    {
        public static CameraUpdate NewLatLng(Position position)
        {
            return new CameraUpdate(position);
        }
    }
}
