## ![Logo](https://raw.githubusercontent.com/themronion/Maui.GoogleMaps/maui/lib/Maui.GoogleMaps/logo.png) Maui.GoogleMaps 

![NuGet](https://img.shields.io/nuget/v/Onion.Maui.GoogleMaps.svg?label=NuGet) ![](https://img.shields.io/nuget/dt/Onion.Maui.GoogleMaps.svg)

Maps library for MAUI that uses Google maps on both mobile platforms - Android and iOS.

Usage is almost the same as [Xamarin.Forms.GoogleMaps - github](https://github.com/amay077/Xamarin.Forms.GoogleMaps) because it was forked from it. More information about library capabilities can be found there. Please note that after the migration to MAUI some properties, events or commands may have been omitted. Also, the namespace is ``Maui.GoogleMaps`` instead of ``Xamarin.Forms.GoogleMaps``. 

## Platform Support

|Platform|Supported|
| ------------------- | :-----------: |
|iOS|Yes|
|Android|Yes|
|Windows 10/11|No|
|Others|No|

## Setup
* Target .NET 8 for best experience
* Install into your MAUI project by downloading the library from nuget: [![NuGet](https://img.shields.io/nuget/v/Onion.Maui.GoogleMaps.svg?label=NuGet)](https://www.nuget.org/packages/Onion.Maui.GoogleMaps/)
* Finish the [Google Cloud Console setup](https://developers.google.com/maps/get-started#create-project)
* Get your API Keys from Google, then in Platforms/Android: 

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

Sample application is here.

* [MauiGoogleMapSample](https://github.com/themronion/Maui.GoogleMaps/tree/maui/sample/MauiGoogleMapSample)

## Contribution

I really appreciate your contribution. If u have spotted a bug you want to fix or have a feature/enhancement you would like to implement please open a pull request targeting the  ``maui`` branch. If u have any questions you are free to reach out to me - all contact info is available in my profile.

## License

See [LICENSE](LICENSE.txt) .

* logo.png by [alecive](http://www.iconarchive.com/show/flatwoken-icons-by-alecive.html) - [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/deed)

