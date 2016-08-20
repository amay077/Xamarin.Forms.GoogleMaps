// Original code from https://github.com/javiholcman/Wapps.Forms.Map/
// Cacheing implemented by Gadzair

using System;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using System.Collections.Generic;
using Java.Nio;
using System.Security.Cryptography;
using Android.Runtime;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps.Android
{
    class Utils
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

        public static Bitmap DrawableToBitmap(Drawable drawable)
        {
            if (drawable is BitmapDrawable)
            {
                return ((BitmapDrawable)drawable).Bitmap;
            }

            int width = drawable.IntrinsicWidth;
            width = width > 0 ? width : 1;
            int height = drawable.IntrinsicHeight;
            height = height > 0 ? height : 1;

            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            drawable.SetBounds(0, 0, canvas.Width, canvas.Height);
            drawable.Draw(canvas);

            return bitmap;
        }

        public static int GetImageResource(string imageName)
        {
            if (imageName.Contains("."))
            {
                imageName = imageName.Substring(0, imageName.IndexOf('.'));
            }

            if (AppResources.DrawableType == null)
            {
                throw new NullReferenceException("You must set Xamarin.Forms.GoogleMaps.Android.AppResources.DrawableType in your Application initialization!");
            }
            return (int)AppResources.DrawableType.GetField(imageName).GetValue(null);
        }

        public static Bitmap ConvertViewToBitmap(global::Android.Views.View v)
        {
            v.SetLayerType(LayerType.Software, null);
            v.DrawingCacheEnabled = true;

            v.Measure(global::Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified),
                global::Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
            v.Layout(0, 0, v.MeasuredWidth, v.MeasuredHeight);

            v.BuildDrawingCache(true);
            Bitmap b = Bitmap.CreateBitmap(v.GetDrawingCache(true));
            v.DrawingCacheEnabled = false; // clear drawing cache
            return b;
        }

        private static LinkedList<string> lruTracker = new LinkedList<string>();
        private static Dictionary<string, global::Android.Gms.Maps.Model.BitmapDescriptor> cache = new Dictionary<string, global::Android.Gms.Maps.Model.BitmapDescriptor>();

        public static Task<global::Android.Gms.Maps.Model.BitmapDescriptor> ConvertViewToBitmapDescriptor(global::Android.Views.View v)
        {
            return Task.Run(() =>
            {
                var bmp = ConvertViewToBitmap(v);
                var buffer = ByteBuffer.Allocate(bmp.ByteCount);
                bmp.CopyPixelsToBuffer(buffer);
                buffer.Rewind();

                // https://forums.xamarin.com/discussion/5950/how-to-convert-from-bitmap-to-byte-without-bitmap-compress
                IntPtr classHandle = JNIEnv.FindClass("java/nio/ByteBuffer");
                IntPtr methodId = JNIEnv.GetMethodID(classHandle, "array", "()[B");
                IntPtr resultHandle = JNIEnv.CallObjectMethod(buffer.Handle, methodId);
                byte[] bytes = JNIEnv.GetArray<byte>(resultHandle);
                JNIEnv.DeleteLocalRef(resultHandle);

                var sha = new SHA1CryptoServiceProvider();
                var hash = Convert.ToBase64String(sha.ComputeHash(bytes));

                var img = global::Android.Gms.Maps.Model.BitmapDescriptorFactory.FromBitmap(bmp);
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
            });
        }

        public static global::Android.Widget.FrameLayout AddViewOnFrameLayout(global::Android.Views.View view, int width, int height)
        {
            var layout = new global::Android.Widget.FrameLayout(view.Context);
            layout.LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);
            view.LayoutParameters = new global::Android.Widget.FrameLayout.LayoutParams(width, height);
            layout.AddView(view);
            return layout;
        }

        public static async Task FixImageSourceOfImageViews(ViewGroup parent)
        {
            if (parent != null)
            {
                for (var i = 0; i < parent.ChildCount; i++)
                {
                    var view = parent.GetChildAt(i);
                    if (view is global::Android.Widget.ImageView)
                    {

                        var imageView = view as global::Android.Widget.ImageView;
                        var imageViewRenderer = imageView.OnFocusChangeListener as ImageRenderer;

                        if (imageViewRenderer.Element.Source is FileImageSource)
                        {
                            var source = imageViewRenderer.Element.Source as FileImageSource;
                            var resId = GetImageResource(source.File);
                            imageView.SetImageResource(resId);
                        }
                    }
                    if (view is ViewGroup)
                    {
                        await FixImageSourceOfImageViews(view as ViewGroup);
                    }
                }
            }
        }

    }
}

