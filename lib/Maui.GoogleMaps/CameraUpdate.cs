using Maui.GoogleMaps.Internals;

namespace Maui.GoogleMaps
{
    public sealed class CameraUpdate
    {
        internal CameraUpdateType UpdateType { get; }
        internal Position Position { get; }
        internal double Zoom { get; }
        internal Bounds Bounds { get; }
        internal int Padding { get; }
        internal CameraPosition CameraPosition { get; }

        internal CameraUpdate(Position position)
        {
            UpdateType = CameraUpdateType.LatLng;
            Position = position;
        }

        internal CameraUpdate(Position position, double zoomLv)
        {
            UpdateType = CameraUpdateType.LatLngZoom;
            Position = position;
            Zoom = zoomLv;
        }

        internal CameraUpdate(Bounds bounds, int padding)
        {
            UpdateType = CameraUpdateType.LatLngBounds;
            Bounds = bounds;
            Padding = padding;
        }

        internal CameraUpdate(CameraPosition cameraPosition)
        {
            UpdateType = CameraUpdateType.CameraPosition;
            CameraPosition = cameraPosition;
        }

    }
}