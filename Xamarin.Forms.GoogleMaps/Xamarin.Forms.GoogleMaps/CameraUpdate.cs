using Xamarin.Forms.GoogleMaps.Internals;

namespace Xamarin.Forms.GoogleMaps
{
    public sealed class CameraUpdate
    {
        internal CameraUpdateType UpdateType { get; }
        internal Position Position { get; }

        internal CameraUpdate(Position position)
        {
            UpdateType = CameraUpdateType.LatLng;
            Position = position;
        }
    }
}