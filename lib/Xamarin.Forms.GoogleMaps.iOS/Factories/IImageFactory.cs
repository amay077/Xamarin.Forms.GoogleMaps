using UIKit;

namespace Xamarin.Forms.GoogleMaps.iOS.Factories
{
    public interface IImageFactory
    {
        UIImage ToUIImage(BitmapDescriptor descriptor);
    }
}