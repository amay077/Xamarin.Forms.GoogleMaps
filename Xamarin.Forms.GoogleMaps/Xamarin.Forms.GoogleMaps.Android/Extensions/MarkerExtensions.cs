using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps.Model;
using Android.Animation;
using Android.Graphics;

namespace Xamarin.Forms.GoogleMaps.Android.Extensions
{
    public static class MarkerExtensions
    {
        public static void AnimateLine(this Marker marker, Position destination, long durationMs)
        {
            if (marker != null)
            {
                Position startPos = marker.Position.ToPosition();

                float startRotation = marker.Rotation;

                ValueAnimator valueAnimator = ValueAnimator.OfFloat(0, 1);
                valueAnimator.SetDuration(durationMs); // duration 1 second
                valueAnimator.AddUpdateListener(new GenericAnimatorUpdateListner(
                    (ValueAnimator animation) =>
                    {
                        try
                        {
                            float v = animation.AnimatedFraction;
                            LatLng newPosition = startPos.TranslateInterpolateLinearFixed(destination, v).ToLatLng();
                            marker.Position = newPosition;
                        }
                        catch (Exception ex)
                        {
                        }
                    }, null
                    ));

                valueAnimator.Start();
            }
        }

        public static void AnimateFadeIn(this Marker marker, long durationMs)
        {
            if (marker != null)
            {
                ValueAnimator valueAnimator = ValueAnimator.OfFloat(0, 1);
                valueAnimator.SetDuration(durationMs); // duration 1 second
                valueAnimator.AddUpdateListener(new GenericAnimatorUpdateListner(
                    (ValueAnimator animation) =>
                    {
                        try
                        {
                            float v = animation.AnimatedFraction;
                            marker.Alpha = v;
                        }
                        catch (Exception ex)
                        {
                        }
                    }, null
                    ));

                valueAnimator.Start();
            }
        }

        public static void AnimateShake(this Marker marker, int shakes, long durationMs)
        {
            double maxAngle = Math.PI / 12;
            double shakeFactor = shakes * 2 * Math.PI;
            if (marker != null)
            {
                ValueAnimator valueAnimator = ValueAnimator.OfFloat(0, 1);
                valueAnimator.SetDuration(durationMs); // duration 1 second
                valueAnimator.AddUpdateListener(new GenericAnimatorUpdateListner(
                    (ValueAnimator animation) =>
                    {
                        try
                        {
                            float v = animation.AnimatedFraction;
                            marker.Rotation = (float) (maxAngle * Math.Sin( v*shakeFactor)).ToDegrees();
                        }
                        catch (Exception ex)
                        {
                        }
                    }, null
                    ));

                valueAnimator.Start();
            }
        }

        public static void AnimatePopIn(this Marker marker, long durationMs)
        {
            // Not possible. Must set custom pin, and resize the bitmap every ticks of the animation
        }

        public static Bitmap Scale(this Bitmap bitmapIn, float scaleFactor)
        {
            Bitmap bitmapOut = Bitmap.CreateScaledBitmap(bitmapIn,
                    (int)Math.Round(bitmapIn.Width * scaleFactor),
                    (int)Math.Round(bitmapIn.Height * scaleFactor), false);

            return bitmapOut;
        }
    }
}