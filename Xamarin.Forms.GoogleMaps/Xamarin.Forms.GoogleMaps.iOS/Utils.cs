using UIKit;
using CoreGraphics;
using Xamarin.Forms.Platform.iOS;
using System.Security.Cryptography;
using System;
using System.Collections.Generic;

namespace Xamarin.Forms.GoogleMaps
{
    internal static class Utils
    {
        public static UIView ConvertFormsToNative(View view, CGRect size)
        {
            var renderer = Platform.iOS.Platform.CreateRenderer(view);

            renderer.NativeView.Frame = size;

            renderer.NativeView.AutoresizingMask = UIViewAutoresizing.All;
            renderer.NativeView.ContentMode = UIViewContentMode.ScaleToFill;

            renderer.Element.Layout(size.ToRectangle());

            var nativeView = renderer.NativeView;

            nativeView.SetNeedsLayout();

            return nativeView;
        }

        private static LinkedList<string> lruTracker = new LinkedList<string>();
        private static Dictionary<string, UIImage> cache = new Dictionary<string, UIImage>();

        public static UIImage ConvertViewToImage(UIView view)
        {
            UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, false, 0);
            view.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            UIImage img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            // Optimization: Let's try to reuse any of the last 10 images we generated
            var bytes = img.AsPNG().ToArray();
            var sha = new SHA1CryptoServiceProvider();
            var hash = Convert.ToBase64String(sha.ComputeHash(bytes));

            var existing = lruTracker.Find(hash);
            if (existing != null)
            {
                lruTracker.Remove(existing);
                lruTracker.AddLast(existing);
                return cache[existing.Value];
            }
            if (lruTracker.Count > 10) // O(1)
            {
                cache.Remove(lruTracker.First.Value);
                lruTracker.RemoveFirst();
            }
            cache.Add(hash, img);
            lruTracker.AddLast(hash);
            return img;
        }
    }
}

