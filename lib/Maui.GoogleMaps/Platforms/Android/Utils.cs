// Original code from https://github.com/javiholcman/Wapps.Forms.Map/
// Cacheing implemented by Gadzair

using Android.Graphics;
using Android.Views;
using Microsoft.Maui.Platform;

namespace Maui.GoogleMaps.Android;

public static class Utils
{
    /// <summary>
    /// convert from dp to pixels
    /// </summary>
    /// <param name="dp">Dp.</param>
    public static int DpToPx(float dp)
    {
        var metrics = global::Android.App.Application.Context.Resources.DisplayMetrics;
        return (int)(dp * metrics.Density);
    }

    /// <summary>
    /// convert from px to dp
    /// </summary>
    /// <param name="px">Px.</param>
    public static float PxToDp(int px)
    {
        var metrics = global::Android.App.Application.Context.Resources.DisplayMetrics;
        return px / metrics.Density;
    }

    public static global::Android.Views.View ConvertMauiToNative(Microsoft.Maui.Controls.View view, IElementHandler handler)
    {
        return ConvertMauiToNative(view, handler.MauiContext);
    }

    public static global::Android.Views.View ConvertMauiToNative(Microsoft.Maui.Controls.View view, IMauiContext mauiContext)
    {
        var nativeView = view.ToPlatform(mauiContext);
        return nativeView;
    }

    public static Bitmap ConvertViewToBitmap(global::Android.Views.View androidView)
    {
        androidView.SetLayerType(LayerType.Hardware, null);
        androidView.DrawingCacheEnabled = true;

        androidView.Measure(
            global::Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified),
            global::Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
        androidView.Layout(0, 0, androidView.MeasuredWidth, androidView.MeasuredHeight);

        androidView.BuildDrawingCache(true);
        Bitmap bitmap = Bitmap.CreateBitmap(androidView.GetDrawingCache(true));
        androidView.DrawingCacheEnabled = false; // clear drawing cache
        return bitmap;
    }

    public static global::Android.Gms.Maps.Model.BitmapDescriptor ConvertViewToBitmapDescriptor(global::Android.Views.View androidView)
    {
        var bitmap = ConvertViewToBitmap(androidView);
        var bitmapDescriptor = global::Android.Gms.Maps.Model.BitmapDescriptorFactory.FromBitmap(bitmap);
        return bitmapDescriptor;
    }

    public static global::Android.Widget.FrameLayout AddViewOnFrameLayout(global::Android.Views.View view, int width, int height)
    {
        var layout = new global::Android.Widget.FrameLayout(view.Context);
        layout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
        view.LayoutParameters = new global::Android.Widget.FrameLayout.LayoutParams(width, height);
        layout.AddView(view);
        return layout;
    }
}