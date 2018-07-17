using System;
using System.IO;

namespace Xamarin.Forms.GoogleMaps
{
    public static class BitmapDescriptorFactory
    {
        public static BitmapDescriptor DefaultMarker(Color color)
        {
            return BitmapDescriptor.DefaultMarker(color, color.GetHashCode().ToString());
        }

        public static BitmapDescriptor FromBundle(string bundleName)
        {
            return BitmapDescriptor.FromBundle(bundleName, bundleName);
        }

        public static BitmapDescriptor FromStream(Stream stream, string id = null)
        {
            return BitmapDescriptor.FromStream(stream, id);
        }

        public static BitmapDescriptor FromView(View view, string id = null)
        {
            return BitmapDescriptor.FromView(view, id);
        }

        //public static BitmapDescriptor FromPath(string absolutePath)
        //{
        //    return BitmapDescriptor.FromPath(absolutePath);
        //}
    }
}

