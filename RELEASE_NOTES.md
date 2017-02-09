Xamarin.Forms.GoogleMaps Release Notes
----
# 1.8.0-beta5

## New Features

* #196 Add Map.InitialCameraUpdate property
* #202 Add Map.CameraPosition property

## Bug Fixes

* #197 [iOS]Fix GroundOverlay.Transparency is incorrect

# 1.8.0-beta4

## New Features

* #7 Add AnimateCamera method

## Bug Fixes

* #192 [UWP]Map.MoveCamera(CameraUpdateFactory.NewBounds(…)) does not reset rotation
* #189 [iOS]CameraPositionChanged does not raise when call MoveCamera with CameraUpdate.FitBounds

# 1.8.0-beta3

## New Features

* #7 Add CameraChanged event
* #7 Add MoveCamera method

# 1.8.0-beta2

## New Features

* #176 Add Map.MyLocationButtonClicked event

# 1.8.0-beta1

## New Features

* #173 Add Pin.IsVisible property

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
