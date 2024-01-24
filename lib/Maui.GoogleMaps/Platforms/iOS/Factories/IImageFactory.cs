using UIKit;

namespace Maui.GoogleMaps.iOS.Factories;

public interface IImageFactory
{
    UIImage ToUIImage(BitmapDescriptor descriptor, IMauiContext mauiContext);
}