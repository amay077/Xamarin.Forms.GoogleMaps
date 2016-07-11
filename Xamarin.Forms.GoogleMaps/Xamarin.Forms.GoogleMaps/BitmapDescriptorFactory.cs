using System;
namespace Xamarin.Forms.GoogleMaps
{
    public static class BitmapDescriptorFactory
    {
        public static BitmapDescriptor DefaultMarker(float hue)
        {
            return new BitmapDescriptor(hue);
        }

        public static BitmapDescriptor FromImage(ImageSource source)
        {
            return new BitmapDescriptor(source);
        }
    }
}

