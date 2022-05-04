using Android.App;
using Android.OS;
using Android.Util;

namespace Xamarin.Forms.GoogleMaps.Android.Extensions
{
    internal static class ActivityExtensions
    {
        public static float GetScaledDensity(this Activity activity) 
        {
            if (Build.VERSION.SdkInt > BuildVersionCodes.R)
            {
                return activity.Resources.Configuration.FontScale;
            }
            else
            {
                var metrics = new DisplayMetrics();
                #pragma warning disable CS0618 // Type or member is obsolete
                activity.Display.GetMetrics(metrics);
                #pragma warning restore CS0618 // Type or member is obsolete
                return metrics.ScaledDensity;
            }

            
        }
    }
}
