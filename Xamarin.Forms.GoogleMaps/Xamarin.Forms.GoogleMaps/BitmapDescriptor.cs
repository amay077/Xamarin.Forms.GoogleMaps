using System;
namespace Xamarin.Forms.GoogleMaps
{
    public sealed class BitmapDescriptor
    {
        internal float Hue { get; }
        internal ImageSource Source { get; }

        internal BitmapDescriptor(float hue)
        {
            this.Hue = hue;
        }

        internal BitmapDescriptor(ImageSource source)
        {
            this.Source = source;
        }
    }
}

