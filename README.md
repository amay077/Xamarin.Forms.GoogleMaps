## ![](logo.png) Xamarin.Forms.GoogleMaps

[日本語の README はこちら！](README-ja.md)

Yet another maps library for Xamarin.Forms that optimized for Google maps.

Usage is almost the same as [Xamarin.Forms.Maps](https://www.nuget.org/packages/Xamarin.Forms.Maps), Because this is forked from [Xamarin.Forms.Maps - github](https://github.com/xamarin/Xamarin.Forms) 

![screenshot](screenshot01.png)

## Motivation

The official [Xamarin.Forms.Map](https://developer.xamarin.com/guides/xamarin-forms/user-interface/map/)  has minumn functions only.

Especially, Bing Maps SDK is very old-fashioned because it has not vector-tile, marker's infowindow.

Android and iOS monopolize most the mobile apps market. Thus I think no need Bing maps support.

Furthermore, I am using Google Maps instead of MapKit because it is easy for define common API for Android and iOS.

**Xamarin.Forms.GoogleMaps provides maximum Google maps features for Xamarin.Forms!!**

## Comparison with Xamarin.Forms.Maps

|Feature|X.F.Maps|X.F.GoogleMaps|
| ------------------- | :-----------: | :-----------: |
|Map types|Yes|Yes|
|Map events|-|Yes|
|Pannning with animation|Yes|Yes|
|Pannning directly|-|Yes|
|Pins|Yes|Yes|
|Custom Pins|-|Yes|
|Pin drag & drop|-|Yes|
|Polygons|-|Yes|
|Lines|-|Yes|
|Circles|-|Yes|
|Custom map tiles|-|Yes|
|Pins binding|-|Yes|
|Pins appearing animation|-|Yes|

For more information, see [Comparison with Xamarin.Forms.Maps](https://github.com/amay077/Xamarin.Forms.GoogleMaps/wiki/Comparison-with-Xamarin.Forms.Maps).

### Setup

* Available on NuGet: https://www.nuget.org/packages/Xamarin.Forms.GoogleMaps/ [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.Geolocator.svg?label=NuGet)](https://www.nuget.org/packages/Xamarin.Forms.GoogleMaps/)
* Install into your PCL project and Client projects.

### Platform Support

|Platform|Supported|
| ------------------- | :-----------: |
|iOS Unified|Yes|
|Android|Yes|
|Windows 10 UWP|Yes (Bing map)|
|Others|No|

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

* ~~Pin.ShowInfoWindow/HideInfoWindow method(or IsVisibleInfoWindow property)~~ add in v1.0.0
* ~~Moving pin by tap and hold~~ add in v1.5.0
* ~~Adding Polygon, Polyline, Circle~~ add in v1.1.0
* and more [enhancements](https://github.com/amay077/Xamarin.Forms.GoogleMaps/labels/enhancement)!

You can use in Windows 10 UWP but this support is reluctant.
Because this library has been determined to optimized for Google Maps, New features will not support in UWP.

### License

See [LICENSE](LICENSE).

* logo.png by [alecive](http://www.iconarchive.com/show/flatwoken-icons-by-alecive.html) - [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/deed)
* Android Icon made by [Hanan](http://www.flaticon.com/free-icon/android_109464) from www.flaticon.com
* Apple Icon made by [Dave Gandy](http://www.flaticon.com/free-icon/apple-logo_25345) from www.flaticon.com

