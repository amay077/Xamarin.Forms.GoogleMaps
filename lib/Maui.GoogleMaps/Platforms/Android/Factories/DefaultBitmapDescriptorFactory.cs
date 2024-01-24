using System.Collections.Concurrent;
using Android.Graphics;
using Android.Widget;
using AndroidBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;
using AndroidBitmapDescriptorFactory = Android.Gms.Maps.Model.BitmapDescriptorFactory;

namespace Maui.GoogleMaps.Android.Factories;

public sealed class DefaultBitmapDescriptorFactory : IBitmapDescriptorFactory
{
    private static readonly Lazy<DefaultBitmapDescriptorFactory> _instance =
        new Lazy<DefaultBitmapDescriptorFactory>(() => new DefaultBitmapDescriptorFactory());

    public static DefaultBitmapDescriptorFactory Instance => _instance.Value;

    private readonly ConcurrentDictionary<string, global::Android.Gms.Maps.Model.BitmapDescriptor> _cacheDictionary = new();

    private DefaultBitmapDescriptorFactory()
    {
    }
    
    public AndroidBitmapDescriptor ToNative(BitmapDescriptor bitmapDescriptor, IMauiContext mauiContext)
    {
        if (bitmapDescriptor.Id != null && _cacheDictionary.TryGetValue(bitmapDescriptor.Id, out var cachedBitmap))
        {
            return cachedBitmap;
        }

        var androidBitmapDescriptor = GetBitmapDescriptor(bitmapDescriptor, mauiContext);
        if (bitmapDescriptor.Id != null)
        {
            _cacheDictionary.TryAdd(bitmapDescriptor.Id, androidBitmapDescriptor);
        }

        return androidBitmapDescriptor;
    }

    private AndroidBitmapDescriptor GetBitmapDescriptor(BitmapDescriptor bitmapDescriptor, IMauiContext mauiContext)
    {
        switch (bitmapDescriptor.Type)
        {
            case BitmapDescriptorType.Default:
                return AndroidBitmapDescriptorFactory.DefaultMarker((float)((bitmapDescriptor.Color.GetHue() * 360f) % 360f));

            case BitmapDescriptorType.Bundle:
                var context = MauiGoogleMaps.Context;
                var resourceId = context.Resources.GetIdentifier(bitmapDescriptor.BundleName, "drawable", context.PackageName);
                return AndroidBitmapDescriptorFactory.FromResource(resourceId);

            case BitmapDescriptorType.Stream:
                var stream = bitmapDescriptor.Stream.Invoke();
                if (stream.CanSeek && stream.Position > 0)
                {
                    stream.Position = 0;
                }

                return AndroidBitmapDescriptorFactory.FromBitmap(BitmapFactory.DecodeStream(stream));

            case BitmapDescriptorType.AbsolutePath:
                return AndroidBitmapDescriptorFactory.FromPath(bitmapDescriptor.AbsolutePath);

            case BitmapDescriptorType.View:
                var iconView = bitmapDescriptor.View.Invoke();
                var nativeView = Utils.ConvertMauiToNative(iconView, mauiContext);
                var androidBitmapDescriptor = Utils.ConvertViewToBitmapDescriptor(nativeView);
                return androidBitmapDescriptor;

            default:
                return AndroidBitmapDescriptorFactory.DefaultMarker();
        }
    }
}