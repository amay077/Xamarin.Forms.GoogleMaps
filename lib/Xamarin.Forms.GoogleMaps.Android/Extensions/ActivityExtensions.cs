using System;
using Android.App;
using Android.Util;

namespace Xamarin.Forms.GoogleMaps.Android.Extensions
{
    internal static class ActivityExtensions
    {
        public static float GetScaledDensity(this Activity self) 
        {
            var metrics = new DisplayMetrics();
            self.WindowManager.DefaultDisplay.GetMetrics(metrics);
            return metrics.ScaledDensity;
        }
    }
}
