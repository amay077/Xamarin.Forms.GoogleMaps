nuget restore Xamarin.Forms.GoogleMaps.sln

msbuild Xamarin.Forms.GoogleMaps.sln /t:Clean;Build /p:Configuration=Release

nuget pack
