## ![](logo.png) Xamarin.Forms.GoogleMaps

[English README is here！](README.md)

Xamarin.Forms 用の Googleマップライブラリです。

[Xamarin.Forms.Maps](https://github.com/xamarin/Xamarin.Forms) をフォークして作っているので、使い方はほとんど同じです。

![screenshot](screenshot01.png)

### なぜこれを作った

BingマップSDKはチープです。ベクトルタイルでもないし、マーカーの吹き出し(InfoWindow
)も無いようです。モバイルアプリの市場のほとんどは Android と iOS なのに、Bingマップのサポートは必要ないように感じます。また、iOS でも Appleマップ よりも Googleマップ を使用した方が、 Android/iOS で共通化しやすいと感じます。

異なる地図SDKで実現可能な最小限の機能しか持たない Xamarin.Forms.Maps に対して、 **同じ Google Maps で多くの共通機能が実現できるのが Xamarin.Forms.GoogleMaps です。**

### セットアップ

* Available on NuGet: https://www.nuget.org/packages/Xamarin.Forms.GoogleMaps/ [![NuGet](https://img.shields.io/nuget/v/Xam.Plugin.Geolocator.svg?label=NuGet)](https://www.nuget.org/packages/Xamarin.Forms.GoogleMaps/)
* PCLプロジェクトと各プラットフォームプロジェクトにインストールしてください

### サポートするプラットフォーム

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

### 使い方

* [Map Control - Xamarin](https://developer.xamarin.com/guides/xamarin-forms/user-interface/map/)
* [Xamarin.Formsで地図を表示するには？（Xamarin.Forms.Maps使用） - Build Insider](http://www.buildinsider.net/mobile/xamarintips/0039)

とほぼ同じです。
初期化メソッドが ``Xamarin.Forms.Maps.Init()`` から ``Xamarin.Forms.GoogleMaps.Init()`` に変更になっています。

iOS の場合、 [Google Maps API for iOS](https://developers.google.com/maps/documentation/ios-sdk/) の API キーを取得し、``AppDelegate.cs`` にて ``Init`` に渡してください。

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

既定の名前空間が ``Xamarin.Forms.Maps`` から ``Xamarin.Forms.GoogleMaps`` に変更されています。他のAPIはすべて同じです。

サンプルプログラムが、

* [XFGoogleMapSample](https://github.com/amay077/Xamarin.Forms.GoogleMaps/tree/master/XFGoogleMapSample)

にあります。

### 今後の予定

なるべく Xamarin.Forms.Maps の API に準じ、Google Maps固有の機能のみ API を追加するつもりです。 

機能要望は、 [@amay077](https://twitter.com/amay077) または、ISSUE やプルリクください！
追加機能案は以下の通りです。

* Pin の InfoWindow の Visible プロパティ
* Pin のタップ＆ホールドによる移動
* Polygon, Polyline, Circle の描画サポート
* etc.. 

Windows 10 UWP 対応は「とりあえず」残しました。
が、基本的には Android/iOS での Google Maps に最適化するので、UWP では未対応の機能が増えると予想されます。  

### ライセンス

[LICENSE](LICENSE) をみて下さい

logo.png by [alecive](http://www.iconarchive.com/show/flatwoken-icons-by-alecive.html) - [CC BY-SA 4.0](https://creativecommons.org/licenses/by-sa/4.0/deed)
