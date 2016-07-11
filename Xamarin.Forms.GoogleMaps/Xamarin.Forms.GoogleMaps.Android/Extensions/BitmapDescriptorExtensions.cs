using System;
using Android.Graphics;
using NativeBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;
using NativeBitmapDescriptorFactory = Android.Gms.Maps.Model.BitmapDescriptorFactory;
using System.Threading;

namespace Xamarin.Forms.GoogleMaps.Android.Extensions
{
    public static class BitmapDescriptorExtensions
    {
        public static NativeBitmapDescriptor ToNative(this BitmapDescriptor self)
        {
            var imageSource = self?.Source;
            if (imageSource != null)
            {
                if (imageSource is StreamImageSource)
                {
                    var sSource = imageSource as StreamImageSource;
                    var cancelToken = new CancellationToken();
                    var task = sSource.Stream.Invoke(cancelToken);
                    task.Wait();
                    return NativeBitmapDescriptorFactory.FromBitmap(BitmapFactory.DecodeStream(task.Result));
                }
                else if (imageSource is UriImageSource)
                {
                    var uriSource = imageSource as UriImageSource;
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}

