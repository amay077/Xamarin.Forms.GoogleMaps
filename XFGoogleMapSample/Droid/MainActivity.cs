using Android.App;
using Android.Content.PM;
using Android.OS;
using Xamarin.Forms.GoogleMaps.Android;

namespace XFGoogleMapSample.Droid
{
    [Activity(Label = "XFGoogleMapSample.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            // Override default BitmapDescriptorFactory by your implementation. 
            var platformConfig = new PlatformConfig
            {
                BitmapDescriptorFactory = new CachingNativeBitmapDescriptorFactory()
            };
            
            Xamarin.FormsGoogleMaps.Init(this, bundle, platformConfig); // initialize for Xamarin.Forms.GoogleMaps
            LoadApplication(new App());
        }
    }
}

