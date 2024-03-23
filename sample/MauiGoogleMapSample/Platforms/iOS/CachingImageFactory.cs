using System.Collections.Concurrent;
using UIKit;
using Maui.GoogleMaps;
using Maui.GoogleMaps.iOS.Factories;

namespace MauiGoogleMapSample
{
    public class CachingImageFactory : IImageFactory
    {
        private readonly ConcurrentDictionary<string, UIImage> _cache = new();

        public UIImage ToUIImage(BitmapDescriptor descriptor, IMauiContext mauiContext)
        {
            var defaultFactory = DefaultImageFactory.Instance;

            if (!string.IsNullOrEmpty(descriptor.Id))
            {
                var cacheEntry = _cache.GetOrAdd(descriptor.Id, _ => defaultFactory.ToUIImage(descriptor, mauiContext));
                return cacheEntry;
            }

            return defaultFactory.ToUIImage(descriptor, mauiContext);
        }
    }
}
