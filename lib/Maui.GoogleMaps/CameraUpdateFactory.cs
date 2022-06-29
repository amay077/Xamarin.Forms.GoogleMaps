
namespace Maui.GoogleMaps
{
    public static class CameraUpdateFactory
    {
        public static CameraUpdate NewPosition(Position position)
        {
            return new CameraUpdate(position);
        }

        public static CameraUpdate NewPositionZoom(Position position, double zoomLv)
        {
            return new CameraUpdate(position, zoomLv);
        }

        public static CameraUpdate NewBounds(Bounds bounds, int padding)
        {
            return new CameraUpdate(bounds, padding);
        }

        public static CameraUpdate NewCameraPosition(CameraPosition cameraPosition)
        {
            return new CameraUpdate(cameraPosition);
        }
    }
}
