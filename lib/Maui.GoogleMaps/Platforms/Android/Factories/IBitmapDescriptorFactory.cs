using AndroidBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;

namespace Maui.GoogleMaps.Android.Factories;

public interface IBitmapDescriptorFactory
{
    AndroidBitmapDescriptor ToNative(BitmapDescriptor bitmapDescriptor, IMauiContext mauiContext);
}