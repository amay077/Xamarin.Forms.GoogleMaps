using System;
namespace Xamarin.Forms.GoogleMaps
{
    public static class CameraUpdateFactory
    {
        public static CameraUpdate NewLatLng(Position position)
        {
            return new CameraUpdate(position);
        }

        public static CameraUpdate NewLatLngZoom(Position position, double zoomLv)
        {
            return new CameraUpdate(position, zoomLv);
        }

        public static CameraUpdate NewLatLngBounds(Bounds bounds, int padding)
        {
            return new CameraUpdate(bounds, padding);
        }

        public static CameraUpdate NewCameraPosition(CameraPosition cameraPosition)
        {
            return new CameraUpdate(cameraPosition);
        }
    }
}
