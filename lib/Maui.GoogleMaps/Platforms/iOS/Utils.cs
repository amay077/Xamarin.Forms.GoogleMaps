// Original code from https://github.com/javiholcman/Wapps.Forms.Map/
// Cacheing implemented by Gadzair

using UIKit;
using CoreGraphics;
using System.Security.Cryptography;
using System.Collections.Concurrent;
using Microsoft.Maui.Platform;

namespace Maui.GoogleMaps
{
    internal static class Utils
    {
        public static UIView ConvertMauiToNative(View view, CGRect size, IElementHandler elementHandler)
        {
            return view.ToPlatform(elementHandler.MauiContext);
        }

        private static LinkedList<string> lruTracker = new LinkedList<string>();
        private static ConcurrentDictionary<string, UIImage> cache = new ConcurrentDictionary<string, UIImage>();

        public static UIImage ConvertViewToImage(UIView view)
        {
            UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, false, 0);
            view.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            UIImage img = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            // Optimization: Let's try to reuse any of the last 10 images we generated
            var bytes = img.AsPNG().ToArray();
            var md5 = MD5.Create();
            var hash = Convert.ToBase64String(md5.ComputeHash(bytes));

            var exists = cache.ContainsKey(hash);
            if (exists)
            {
                lruTracker.Remove(hash);
                lruTracker.AddLast(hash);
                return cache[hash];
            }
            if (lruTracker.Count > 10) // O(1)
            {
                UIImage tmp;
                cache.TryRemove(lruTracker.First.Value, out tmp);
                lruTracker.RemoveFirst();
            }
            cache.GetOrAdd(hash, img);
            lruTracker.AddLast(hash);
            return img;
        }
    }
}

