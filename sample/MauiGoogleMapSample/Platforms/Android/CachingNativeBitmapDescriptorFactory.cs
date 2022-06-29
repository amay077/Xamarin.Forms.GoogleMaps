using System.Collections.Concurrent;
using Maui.GoogleMaps.Android.Factories;
using Maui.GoogleMaps;
using AndroidBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;

namespace MauiGoogleMapSample
{
    public sealed class CachingNativeBitmapDescriptorFactory : IBitmapDescriptorFactory
    {
        private readonly ConcurrentDictionary<string, AndroidBitmapDescriptor> _cache = new();
        public AndroidBitmapDescriptor ToNative(BitmapDescriptor descriptor)
        {
            var defaultFactory = DefaultBitmapDescriptorFactory.Instance;
            
            if (!string.IsNullOrEmpty(descriptor.Id))
            {
                var cacheEntry = _cache.GetOrAdd(descriptor.Id, _ => defaultFactory.ToNative(descriptor));
                return cacheEntry;
            }

            return defaultFactory.ToNative(descriptor);
        }
    }
}