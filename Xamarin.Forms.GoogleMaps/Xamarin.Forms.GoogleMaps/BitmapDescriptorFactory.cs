using System;
namespace Xamarin.Forms.GoogleMaps
{
    public static class BitmapDescriptorFactory
    {
        public static BitmapDescriptor DefaultMarker(Color color)
        {
            return new BitmapDescriptor(BitmapDescriptorType.Default, color);
        }

        public static BitmapDescriptor FromBundle(string bundleName)
        {
            return new BitmapDescriptor(BitmapDescriptorType.Bundle, bundleName);
        }
    }
}

