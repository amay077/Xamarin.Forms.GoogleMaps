using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xamarin.Forms.GoogleMaps.iOS;

namespace XFGoogleMapSample.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            // Override default ImageFactory by your implementation. 
            var platformConfig = new PlatformConfig
            {
                ImageFactory = new CachingImageFactory()
            };

            Xamarin.FormsGoogleMaps.Init(Variables.GOOGLE_MAPS_IOS_API_KEY, platformConfig); 
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}

