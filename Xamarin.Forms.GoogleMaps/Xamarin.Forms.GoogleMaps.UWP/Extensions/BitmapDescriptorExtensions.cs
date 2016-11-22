using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms.GoogleMaps;

namespace Xamarin.Forms.GoogleMaps.Extensions.UWP
{
    internal static class BitmapDescriptorExtensions
    {
        public static BitmapImage ToBitmapDescriptor(this BitmapDescriptor self)
        {
            switch (self.Type)
            {
                case BitmapDescriptorType.Default:
                    //TODO:
                    return new BitmapImage();
                case BitmapDescriptorType.Bundle:
                    return new BitmapImage(new Uri(self.BundleName));
                case BitmapDescriptorType.Stream:
                    var bitmap = new BitmapImage();
                    var memoryStream = new MemoryStream();
                    self.Stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                    bitmap.SetSource(memoryStream.AsRandomAccessStream());
                    return bitmap;
                case BitmapDescriptorType.AbsolutePath:
                    return new BitmapImage(new Uri(self.AbsolutePath));
                default:
                    //TODO
                    return new BitmapImage();
            }
        }
    }
}
