using System;
using UIKit;
using Xamarin.Forms.Platform.iOS;
namespace Xamarin.Forms.GoogleMaps.iOS.Extensions
{
    internal static class BitmapDescriptorExtensions
    {
        public static UIImage ToUIImage(this BitmapDescriptor self)
        {
            switch (self.Type)
            {
                case BitmapDescriptorType.Default:
                    return Google.Maps.Marker.MarkerImage(self.Color.ToUIColor());
                case BitmapDescriptorType.Bundle:
                    return UIImage.FromBundle(self.BundleName);
                default:
                    return Google.Maps.Marker.MarkerImage(UIColor.Red);
            }
        }
    }
}

