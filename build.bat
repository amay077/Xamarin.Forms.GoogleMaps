nuget restore Xamarin.Forms.GoogleMaps.sln

msbuild Xamarin.Forms.GoogleMaps.sln /t:Clean
msbuild Xamarin.Forms.GoogleMaps.sln /t:Build /p:Configuration=Release

nuget pack
