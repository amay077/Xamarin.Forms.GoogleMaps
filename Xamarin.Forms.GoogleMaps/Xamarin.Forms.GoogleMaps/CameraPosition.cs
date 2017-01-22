using System;

using Xamarin.Forms;

namespace Xamarin.Forms.GoogleMaps
{
    public sealed class CameraPosition
    {
        public Position Target { get; }
        public double Bearing { get; }
        public double Tilt { get; }
        public double Zoom { get; }

        internal CameraPosition(Position target, double bearing, double tilt, double zoom)
        {
            Target = target;
            Bearing = bearing;
            Tilt = tilt;
            Zoom = zoom;
        }
    }

}
