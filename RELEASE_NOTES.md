Xamarin.Forms.GoogleMaps Release Notes
----
# 2.1.0-beta7

## New Features

* #331 [Android/iOS]Add Polygon.ZIndex and Circle.ZIndex property 
* PR#342 [UWP]Support SelectedPinChanged

## Bug Fixes

* #310 [Android/iOS]Fix MethodAccessException in CameraUpdateConverter
* #335 [Android/iOS]Fix HasScrollEnabled not working correctly

# 2.1.0-beta6

## New Features

* #313 [Android/iOS/UWP]Add MapType Terrain
* #325 [Android/iOS/UWP]Add Polyline.ZIndex property
* #327 [UWP]Support Pin.InfoWindowClicked event
* #326 [UWP]Support Polyline

## Bug Fixes

* PR#318 [UWP]Fix memory leak when remove pins
* #332 [iOS]Changes Polyline and Polygon property does not work at after added

# 2.1.0-beta5

## New Features

* #117 [Android/iOS]Support map styling

# 2.1.0-beta4

## New Features

* #17 [Android/iOS]Add Circle.Clicked event(Fix missing some implementations)

# 2.1.0-beta3

## New Features

* #17 [Android/iOS]Add Circle.Clicked event 

# 2.1.0-beta2

## New Features

* #8 Add UiSettings class

# 2.1.0-beta1

## New Features

* #308 [Android/iOS]Map.InfoWindowLongClicked event

# 2.0.0

## **Important**

* Support Xamarin.Forms 2.3.4+ and Xamarin.GooglePlayServices.Maps 42+
* End of support for Xamarin.Forms 2.3.3- and Xamarin.GooglePlayServices.Maps 29-# 1.8.2-beta1

## New Features

* #264 [Android/iOS]Add Pin.ZIndex property

## Bug Fixes

* #280 [Android]Build with Link All SDK Assemblies Fails

# 1.8.1

## Bug Fixes

* #278 [iOS]HasRotationEnabled doesn't work
* PR#286 [Android/iOS/UWP]Fix general Memory leaks
* #281 [iOS]Fix Null Reference when navigating away from map page
* PR#279 [iOS]Fixing stream position 

# 1.8.0

## New Features

* #63 [Android/iOS]Map.IndoorEnabled property
* #181 [Android/iOS]Add Polygon.Holes property
* #234 [Android/iOS]Add Pin.InfoWindowAnchor property
* #241 [Android/iOS]Map.TakeSnapshot method
* #255 [Android/iOS]Add Pin.Flat property
* PR#257 [UWP]Support Map.HasRotationEnabled
* #71 [iOS/Android]Add Map.Padding property
* #84 [iOS/Android]Add Map.HasRotationEnabled property
* #196 Add Map.InitialCameraUpdate property
* #202 Add Map.CameraPosition property
* #7 Add AnimateCamera method
* #7 Add CameraChanged event
* #7 Add MoveCamera method
* #176 Add Map.MyLocationButtonClicked event
* #173 Add Pin.IsVisible property

## Bug Fixes

* #246 [Android]NullPointerException occurs when reused BitmapDescriptorFactory.FromStream()
* PR#258 [UWP]CameraPosition Arguments were in wrong order
* #231 Fix Pin.IsVisible does not used when native marker created
* #216 [UWP]Fix Map.VisibleRegion is null when first CameraChanged
* #205 [iOS/Android] Move Map camera to correct region on layout change(from XF.Maps)
* #197 [iOS]Fix GroundOverlay.Transparency is incorrect
* #192 [UWP]Map.MoveCamera(CameraUpdateFactory.NewBounds(…)) does not reset rotation
* #189 [iOS]CameraPositionChanged does not raise when call MoveCamera with CameraUpdate.FitBounds

# 1.7.1

## New Features

* #153 Supports XAML Previewer for Visual Stuidio/Xamarin Studio
* PR#137 Change default bindingmode on SelectedPin to TwoWay
* PR#166 [UWP]Supports InfoWindow
* PR#136 [UWP]Supports TileLayers
* #134 Add InfoWindowClicked event 
* PR#127 [UWP]Supports EnableTraffic

## Bug Fixes

* #148 java.Lang.IllegalArgumentException: hue outside range
* #160 Center of MapRegion on iOS seems not right

# 1.7.0

## New Features

* #78 Add Map.PinClicked event and can handling pin selection yourself. 
* #80 Disable MapToolbar in Android 
* PR#98 Add some convenient methods to Bounds and MapSpan
* #103 Add traffic layer support
* #95 pin rotation support

## Bug Fixes

* #89 Fix No constructor found for Xamarin.Forms.GoogleMaps.Android.MapRenderer::.ctor(System.IntPtr, Android.Runtime.JniHandleOwnership)

## **Notice**

* Pin.Clicked event is now obsolete. Please use Map.PinClicked instead of this.

# 1.6.1

## New Features

* #82 Add MapType.None that displays empty map

## Bug Fixes

* #73 [Android]get_Map returns NULL POINTER EXCEPTION(Support Google Play-services ver.32)
* [Android]Set null to Pin.Icon is does not work

# 1.6.0

## New Features

* PR#62 Generation of Pin Icon from Xamarin Views
* #52 Support for Ground Overlays

## Bug Fixes

* #30 Map.Pins.Add doesn't work when page OnAppearing
* #68 [Android]Duplicate Pin when Pins.Add in OnAppearing 

# 1.5.0

## New Features

* #10 Moving pin by tap and hold (Android / iOS only)
* #45 Support Map.MapClicked and MapLongClicked event (Android / iOS only)

# 1.4.1

## Bug Fixes

* #42 [iOS]Geocoder not supoort

# 1.4.0

## New Features

* #33 Add custom marker support (Android / iOS only)
* #9 Add Pin and Shapes BindableProperty support (Android / iOS only)

## Bug Fixes

* #38 [iOS] Exception: Specified cast is not valid

# 1.3.0

## New Features

* Add TileLayer support (Android / iOS only)

# 1.2.0

## New Features

* Add Map.SelectedPinChanged event

## Bug Fixes

* #16 [Android]Map.SelectedPin does not clear when tap pin to hide info-window 

# 1.1.1

## Bug Fixes

* #12 [Android]Selecting pin from logic causes error
* #14 Some combinations of set Map.SelectedPin, tap pin and Map.Pins.Remove(Clear) causes strange behavior 

# 1.1.0

## New Features

* Add Polyline support (Android / iOS only)
* Add Polygon support (Android / iOS only)
* Add Circle support (Android / iOS only)
* Add Tag property to Pin, Polyline, Polygon, Circle classses
* Add MapSpan.FromPositions method
* Add animate parameter to Map.MoveToRegion method

end of contents
