using UIKit;
using Microsoft.Maui.Platform;

namespace Maui.GoogleMaps;

internal static class Utils
{
    public static UIView ConvertMauiToNative(View view, IElementHandler elementHandler)
    {
        return ConvertMauiToNative(view, elementHandler.MauiContext);
    }

    public static UIView ConvertMauiToNative(View view, IMauiContext mauiContext)
    {
        return view.ToPlatform(mauiContext);
    }

    public static UIImage ConvertViewToImage(UIView view)
    {
        UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, false, 0);
        view.Layer.RenderInContext(UIGraphics.GetCurrentContext());
        UIImage img = UIGraphics.GetImageFromCurrentImageContext();
        UIGraphics.EndImageContext();

        return img;
    }
}