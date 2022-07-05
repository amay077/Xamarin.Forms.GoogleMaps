## ![](logo.png) Xamarin.Forms.GoogleMaps 

![](https://img.shields.io/nuget/v/Onion.Maui.GoogleMaps.svg) ![](https://img.shields.io/nuget/dt/Onion.Maui.GoogleMaps.svg)

Maps library for MAUI that optimized for Google maps.

Usage is almost the same as [Xamarin.Forms.Maps](https://www.nuget.org/packages/Xamarin.Forms.Maps), Because this is forked from [Xamarin.Forms.GoogleMaps - github](https://github.com/amay077/Xamarin.Forms.GoogleMaps) which was forked before from [Xamarin.Forms.Maps - github](https://github.com/xamarin/Xamarin.Forms)

## Comparison with Xamarin.Forms.Maps

For more information, see [Comparison with Xamarin.Forms.Maps](https://github.com/amay077/Xamarin.Forms.GoogleMaps/wiki/Comparison-with-Xamarin.Forms.Maps).

## Setup

* Available on NuGet: https://www.nuget.org/packages/Onion.Maui.GoogleMaps/ [![NuGet](https://img.shields.io/nuget/v/Onion.Maui.GoogleMaps.svg?label=NuGet)](https://www.nuget.org/packages/Onion.Maui.GoogleMaps/)
* Install into your MAUI project

## Platform Support

|Platform|Supported|
| ------------------- | :-----------: |
|iOS|Yes|
|Android|Yes|
|Windows 10/11|No|
|Others|No|

## Usage

Same as this

* [Map Control - Xamarin](https://developer.xamarin.com/guides/xamarin-forms/user-interface/map/)

Get your API Keys from Google, then in Platforms/Android: 

```csharp
// MainApplication.cs
 [Application]
 [MetaData("com.google.android.maps.v2.API_KEY",
            Value = Variables.GOOGLE_MAPS_ANDROID_API_KEY)]
 public class MainApplication : MauiApplication
 {
    public MainApplication(IntPtr handle, JniHandleOwnership ownership) 
    : base(handle, ownership)
    { }
    
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
  }
```

And in the MauiProgram.cs file

```csharp
// MauiProgram.cs
using Maui.GoogleMaps.Hosting;
...
public static MauiApp CreateMauiApp()
{
        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
            
#if ANDROID
        builder.UseGoogleMaps();
#elif IOS
        builder.UseGoogleMaps(Variables.GOOGLE_MAPS_IOS_API_KEY);
#endif
        return builder.Build();	
}
```

Namespace is ``Maui.GoogleMaps`` instead of ``Xamarin.Forms.GoogleMaps``. 

Sample application is here.

* [MauiGoogleMapSample](https://github.com/themronion/Maui.GoogleMaps/tree/maui/sample/MauiGoogleMapSample)

## Contribution

We really appreciate your contribution.

Please read the [contribution guideline](CONTRIBUTING.md).

## License

See [LICENSE](LICENSE.txt) .

* logo.png by [alecive](http://www.iconarchive.com/show/flatwoken-icons-by-alecive.html) - [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/deed)
* Android Icon made by [Hanan](http://www.flaticon.com/free-icon/android_109464) from www.flaticon.com
* Apple Icon made by [Dave Gandy](http://www.flaticon.com/free-icon/apple-logo_25345) from www.flaticon.com

