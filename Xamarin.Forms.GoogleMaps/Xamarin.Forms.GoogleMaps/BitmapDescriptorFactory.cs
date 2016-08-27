using System;
using System.IO;

namespace Xamarin.Forms.GoogleMaps
{
    public static class BitmapDescriptorFactory
    {
        public static BitmapDescriptor DefaultMarker(Color color)
        {
            return BitmapDescriptor.DefaultMarker(color);
        }

        public static BitmapDescriptor FromBundle(string bundleName)
        {
            return BitmapDescriptor.FromBundle(bundleName);
        }

        public static BitmapDescriptor FromStream(Stream stream)
        {
            return BitmapDescriptor.FromStream(stream);
        }

        public static BitmapDescriptor FromView(View view)
        {
            return BitmapDescriptor.FromView(view);
        }

        //public static BitmapDescriptor FromPath(string absolutePath)
        //{
        //    return BitmapDescriptor.FromPath(absolutePath);
        //}
    }
}

