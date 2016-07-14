using System;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;
using Foundation;
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
                case BitmapDescriptorType.Stream:
                    return UIImage.LoadFromData(NSData.FromStream(self.Stream));
                case BitmapDescriptorType.AbsolutePath:
                    return UIImage.FromFile(self.AbsolutePath);
                default:
                    return Google.Maps.Marker.MarkerImage(UIColor.Red);
            }
        }
    }
}

