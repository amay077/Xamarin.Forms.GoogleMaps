Xamarin.Forms.GoogleMaps Release Notes
----

# 1.7.0-beta1

## New Features

* #78 Add Map.PinClicked event and can handling pin selection yourself. 

## **Notice**

* Pin.Clicked event is not obsolete. Please use Map.PinClicked instead of this.

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
