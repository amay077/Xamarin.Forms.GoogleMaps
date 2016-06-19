using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace XFGoogleMapSample.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            Xamarin.FormsGoogleMaps.Init("your_api_key"); // initialize for Xamarin.Forms.GoogleMaps
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}

