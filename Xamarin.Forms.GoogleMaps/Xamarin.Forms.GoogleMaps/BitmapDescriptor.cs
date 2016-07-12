using System;
namespace Xamarin.Forms.GoogleMaps
{
    public sealed class BitmapDescriptor
    {
        internal BitmapDescriptorType Type { get; }
        internal Color Color { get; }
        internal string BundleName { get; }

        internal BitmapDescriptor(BitmapDescriptorType type, Color color)
        {
            this.Type = type;
            this.Color = color;
        }

        internal BitmapDescriptor(BitmapDescriptorType type, string fileName)
        {
            this.Type = type;
            this.BundleName = fileName;
        }
    }
}

