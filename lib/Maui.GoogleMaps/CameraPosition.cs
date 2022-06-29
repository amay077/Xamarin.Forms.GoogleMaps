
namespace Maui.GoogleMaps
{
    public sealed class CameraPosition
    {
        public Position Target { get; }
        public double Bearing { get; }
        public double Tilt { get; }
        public double Zoom { get; }

        public CameraPosition(Position target, double zoom)
        {
            Target = target;
            Zoom = zoom;
        }

        public CameraPosition(Position target, double zoom, double bearing)
        {
            Target = target;
            Zoom = zoom;
            Bearing = bearing;
        }

        public CameraPosition(Position target, double zoom, double bearing, double tilt)
        {
            Target = target;
            Bearing = bearing;
            Tilt = tilt;
            Zoom = zoom;
        }
    }

}
