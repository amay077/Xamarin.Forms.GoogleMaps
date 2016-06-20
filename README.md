## ![](logo.png) Xamarin.Forms.GoogleMaps

[日本語の README はこちら！](README-ja.md)

Yet another maps library for Xamarin.Forms that optimized for Google maps.

Usage is almost the same as [Xamarin.Forms.Maps](https://www.nuget.org/packages/Xamarin.Forms.Maps), Because this is forked from [Xamarin.Forms.Maps - github](https://github.com/xamarin/Xamarin.Forms) 

![screenshot](screenshot01.png)

### Setup

* Available on NuGet: https://www.nuget.org/packages/Xamarin.Forms.GoogleMaps/ [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.Geolocator.svg?label=NuGet)](https://www.nuget.org/packages/Xamarin.Forms.GoogleMaps/)
* Install into your PCL project and Client projects.

### Platform Support

|Platform|Supported|
| ------------------- | :-----------: |
|iOS Classic|No|
|iOS Unified|Yes|
|Android|Yes|
|Windows Phone Silverlight|No|
|Windows Phone RT|No|
|Windows Store RT|No|
|Windows 10 UWP|Yes (Bing map)|
|Xamarin.Mac|No|

### Usage

Same as this

* [Map Control - Xamarin](https://developer.xamarin.com/guides/xamarin-forms/user-interface/map/)

In iOS, get the API Key from [Google Maps API for iOS](https://developers.google.com/maps/documentation/ios-sdk/) then insert ``Init`` of ``AppDelegate.cs``.  

```csharp
[Register("AppDelegate")]
public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
{
    public override bool FinishedLaunching(UIApplication app, NSDictionary options)
    {
        global::Xamarin.Forms.Forms.Init();
        Xamarin.FormsGoogleMaps.Init("your_api_key");
        LoadApplication(new App());

        return base.FinishedLaunching(app, options);
    }
}
``` 

Namespace is ``Xamarin.Forms.GoogleMaps`` instead of ``Xamarin.Forms.Maps``. 

Sample application is here.

* [XFGoogleMapSample](https://github.com/amay077/Xamarin.Forms.GoogleMaps/tree/master/XFGoogleMapSample)

### Future plans

I will follow Xamarin.Forms.Maps API as possible. I will add new API only when I implement Google maps original feature.

If you have proposals then send to [@amay077](https://twitter.com/amay077) or submit ISSUE or Pull-request!

Latest scheduled features as follows:

* Pin.ShowInfoWindow/HideInfoWindow method(or IsVisibleInfoWindow property)
* Moving pin by tap and hold
* Adding Polygon, Polyline, Circle
* etc.. 

You can use in Windows 10 UWP but this support is reluctant.
Because this library has been determined to optimized for Google Maps, New features will not support in UWP.

### License

See [LICENSE](LICENSE).

logo.png by [alecive](http://www.iconarchive.com/show/flatwoken-icons-by-alecive.html) - [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/deed)
