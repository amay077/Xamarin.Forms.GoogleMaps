using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using Android.Gms.Maps.Model;
//using Android.Animation;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.iOS.Extensions;

/*
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
            }
            ));

        valueAnimator.Start();
    }
}

}
}*/
