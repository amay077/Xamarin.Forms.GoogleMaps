using Android.App;
using Android.Runtime;

namespace MauiGoogleMapSample
{
    [Application]
    [MetaData("com.google.android.maps.v2.API_KEY",
             Value = Variables.GOOGLE_MAPS_ANDROID_API_KEY)]
    public class MainApplication : MauiApplication
    {
       
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}