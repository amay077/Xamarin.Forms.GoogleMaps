using System;
using System.Collections.Concurrent;
using UIKit;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.GoogleMaps.iOS.Factories;

namespace XFGoogleMapSample.iOS
{
    public class CachingImageFactory : IImageFactory
    {
        private readonly ConcurrentDictionary<string, UIImage> _cache
            = new ConcurrentDictionary<string, UIImage>();

        public UIImage ToUIImage(BitmapDescriptor descriptor)
        {
            var defaultFactory = DefaultImageFactory.Instance;

            if (!string.IsNullOrEmpty(descriptor.Id))
            {
                var cacheEntry = _cache.GetOrAdd(descriptor.Id, _ => defaultFactory.ToUIImage(descriptor));
                return cacheEntry;
            }

            return defaultFactory.ToUIImage(descriptor);
        }
    }
}
