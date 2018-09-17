using System;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.GoogleMaps.iOS.Factories
{
    // ReSharper disable once InconsistentNaming
    public sealed class DefaultImageFactory : IImageFactory
    {
        private static readonly Lazy<DefaultImageFactory> _instance
            = new Lazy<DefaultImageFactory>(() => new DefaultImageFactory());

        public static DefaultImageFactory Instance
        {
            get { return _instance.Value; }
        }
        
        private DefaultImageFactory()
        {
        }
        
        public UIImage ToUIImage(BitmapDescriptor descriptor)
        {
            switch (descriptor.Type)
            {
                case BitmapDescriptorType.Default:
                    return Google.Maps.Marker.MarkerImage(descriptor.Color.ToUIColor());
                case BitmapDescriptorType.Bundle:
                    return UIImage.FromBundle(descriptor.BundleName);
                case BitmapDescriptorType.Stream:
                    descriptor.Stream.Position = 0;
                    // Resize to screen scale
                    return UIImage.LoadFromData(NSData.FromStream(descriptor.Stream), UIScreen.MainScreen.Scale);
                case BitmapDescriptorType.AbsolutePath:
                    return UIImage.FromFile(descriptor.AbsolutePath);
                default:
                    return Google.Maps.Marker.MarkerImage(UIColor.Red);
            }
        }
    }
}