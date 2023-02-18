using System;
using Android.App;
using Android.Runtime;

namespace XFGoogleMapSample.Droid
{
    [Application]
    [MetaData("com.google.android.geo.API_KEY", Value = Variables.GOOGLE_MAPS_ANDROID_API_KEY)]
    [MetaData("com.google.android.gms.version", Value = "@integer/google_play_services_version")]
    [UsesLibrary("org.apache.http.legacy", required: false)]
    public class MyApp : Application
    {
        public MyApp(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }
    }
}

