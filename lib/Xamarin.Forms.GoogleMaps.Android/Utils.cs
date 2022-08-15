using System;
using Android.Graphics;
using Android.Views;
using Xamarin.Forms.Platform.Android;
using System.Collections.Generic;
using Java.Nio;
using System.Security.Cryptography;
using Android.Runtime;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using static Android.Graphics.Bitmap;

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

        public static Task<global::Android.Views.View> ConvertFormsToNative(View view, Rectangle size, IVisualElementRenderer vRenderer)
        {
            return Task.Run(() =>
            {
                var viewGroup = vRenderer.View;
                vRenderer.Tracker.UpdateLayout();
                var layoutParams = new ViewGroup.LayoutParams((int)size.Width, (int)size.Height);
                viewGroup.LayoutParameters = layoutParams;
                view.Layout(size);
                viewGroup.Layout(0, 0, (int)view.WidthRequest, (int)view.HeightRequest);
                return viewGroup;
            });
        }

        public static Bitmap ConvertViewToBitmap(global::Android.Views.View view)
        {
            view.SetLayerType(LayerType.Hardware, null);

            view.Measure(global::Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified),
                         global::Android.Views.View.MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified));
            view.Layout(0, 0, view.MeasuredWidth, view.MeasuredHeight);

            var bitmap =  CreateBitmap(view.MeasuredWidth, view.MeasuredHeight, Config.Argb8888);

            var canvas = new Canvas(bitmap);

            view.Draw(canvas);

            return bitmap;
        }

        private static LinkedList<string> lruTracker = new LinkedList<string>();
        private static ConcurrentDictionary<string, global::Android.Gms.Maps.Model.BitmapDescriptor> cache = new ConcurrentDictionary<string, global::Android.Gms.Maps.Model.BitmapDescriptor>();

        public static Task<global::Android.Gms.Maps.Model.BitmapDescriptor> ConvertViewToBitmapDescriptor(global::Android.Views.View v)
        {
            return Task.Run(() => {

                var bmp = ConvertViewToBitmap(v);
                var img = global::Android.Gms.Maps.Model.BitmapDescriptorFactory.FromBitmap(bmp);

                var buffer = ByteBuffer.Allocate(bmp.ByteCount);
                bmp.CopyPixelsToBuffer(buffer);
                buffer.Rewind();

                // https://forums.xamarin.com/discussion/5950/how-to-convert-from-bitmap-to-byte-without-bitmap-compress
                IntPtr classHandle = JNIEnv.FindClass("java/nio/ByteBuffer");
                IntPtr methodId = JNIEnv.GetMethodID(classHandle, "array", "()[B");
                IntPtr resultHandle = JNIEnv.CallObjectMethod(buffer.Handle, methodId);
                byte[] bytes = JNIEnv.GetArray<byte>(resultHandle);
                JNIEnv.DeleteLocalRef(resultHandle);

                var sha = MD5.Create();
                var hash = Convert.ToBase64String(sha.ComputeHash(bytes));

                var exists = cache.ContainsKey(hash);
                lock (lruTracker)
                {//LinkedList is not thread safe impl, will crash in multi-trheads scenerios, and so using lock to work-around
                    if (exists)
                    {
                        lruTracker.Remove(hash);
                        lruTracker.AddLast(hash);
                        return cache[hash];
                    }
                    if (lruTracker.Count > 10) // O(1)
                    {
                        global::Android.Gms.Maps.Model.BitmapDescriptor tmp;
                        cache.TryRemove(lruTracker.First.Value, out tmp);
                        lruTracker.RemoveFirst();
                    }
                    lruTracker.AddLast(hash);
                }//lock lruTracker
                cache.GetOrAdd(hash, img);
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

    }
}

