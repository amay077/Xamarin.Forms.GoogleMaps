using AndroidBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;

namespace Xamarin.Forms.GoogleMaps.Android.Factories
{
    public interface IBitmapDescriptorFactory
    {
        AndroidBitmapDescriptor ToNative(BitmapDescriptor descriptor);
    }
}