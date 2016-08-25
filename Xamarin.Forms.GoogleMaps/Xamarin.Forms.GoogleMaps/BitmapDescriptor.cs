using System;
using System.IO;

namespace Xamarin.Forms.GoogleMaps
{
    public sealed class BitmapDescriptor
    {
        internal BitmapDescriptorType Type { get; private set; }
        internal Color Color { get; private set; }
        internal string BundleName { get; private set; }
        internal Stream Stream { get; private set; }
        internal string AbsolutePath { get; private set; }
        internal View View { get; private set; }

        private BitmapDescriptor()
        {
        }

        internal static BitmapDescriptor DefaultMarker(Color color)
        {
            return new BitmapDescriptor()
            {
                Type = BitmapDescriptorType.Default,
                Color = color
            };
        }

        internal static BitmapDescriptor FromBundle(string bundleName)
        {
            return new BitmapDescriptor()
            {
                Type = BitmapDescriptorType.Bundle,
                BundleName = bundleName
            };
        }

        internal static BitmapDescriptor FromStream(Stream stream)
        {
            return new BitmapDescriptor()
            {
                Type = BitmapDescriptorType.Stream,
                Stream = stream
            };
        }

        internal static BitmapDescriptor FromPath(string absolutePath)
        {
            return new BitmapDescriptor()
            {
                Type = BitmapDescriptorType.AbsolutePath,
                AbsolutePath = absolutePath
            };
        }

        internal static BitmapDescriptor FromView(View view)
        {
            return new BitmapDescriptor()
            {
                Type = BitmapDescriptorType.View,
                View = view
            };
        }
    }
}

