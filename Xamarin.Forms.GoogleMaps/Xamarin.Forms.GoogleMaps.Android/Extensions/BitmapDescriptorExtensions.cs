using System;
using Android.Graphics;
using NativeBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;
using NativeBitmapDescriptorFactory = Android.Gms.Maps.Model.BitmapDescriptorFactory;
using System.Threading;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Forms.GoogleMaps.Android.Extensions
{
    internal static class BitmapDescriptorExtensions
    {
        public static NativeBitmapDescriptor ToBitmapDescriptor(this BitmapDescriptor self)
        {
            switch (self.Type)
            {
                case BitmapDescriptorType.Default:
                    return NativeBitmapDescriptorFactory.DefaultMarker((float)self.Color.Hue * 360f);
                case BitmapDescriptorType.Bundle:
                    return NativeBitmapDescriptorFactory.FromAsset(self.BundleName);
                case BitmapDescriptorType.Stream:
                    return NativeBitmapDescriptorFactory.FromBitmap(BitmapFactory.DecodeStream(self.Stream));
                case BitmapDescriptorType.AbsolutePath:
                    return NativeBitmapDescriptorFactory.FromPath(self.AbsolutePath);
                default:
                    return NativeBitmapDescriptorFactory.DefaultMarker();
            }
        }
    }
}

