using System;
using Android.App;
using Android.Runtime;

namespace XFGoogleMapSample.Droid
{
    [Android.App.Application]
    [MetaData("com.google.android.maps.v2.API_KEY",
              Value = "your_google_map_key")]
    public class MyApp : Android.App.Application
    {
        public MyApp(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }
    }
}

