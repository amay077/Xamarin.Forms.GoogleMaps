using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    public partial class BasicMapPage : ContentPage
    {
        public BasicMapPage()
        {
            InitializeComponent();

            // MapTypes
            var mapTypeValues = new List<MapType>();
            foreach (var mapType in Enum.GetValues(typeof(MapType)))
            {
                mapTypeValues.Add((MapType)mapType);
                pickerMapType.Items.Add(Enum.GetName(typeof(MapType), mapType));
            }

            pickerMapType.SelectedIndexChanged += (sender, e) =>
            {
                map.MapType = mapTypeValues[pickerMapType.SelectedIndex];
            };
            pickerMapType.SelectedIndex = 0;

            // MyLocationEnabled
            switchMyLocationEnabled.Toggled += (sender, e) =>
            {
                map.MyLocationEnabled = e.Value;
            };
            switchMyLocationEnabled.IsToggled = map.MyLocationEnabled;

            // IsTrafficEnabled
            switchIsTrafficEnabled.Toggled += (sender, e) =>
            {
                map.IsTrafficEnabled = e.Value;
            };
            switchIsTrafficEnabled.IsToggled = map.IsTrafficEnabled;

            // IndoorEnabled
            switchIndoorEnabled.Toggled += (sender, e) =>
            {
                map.IsIndoorEnabled = e.Value;
            };
            switchIndoorEnabled.IsToggled = map.IsIndoorEnabled;

            // CompassEnabled
            switchCompassEnabled.Toggled += (sender, e) =>
            {
                map.UiSettings.CompassEnabled = e.Value;
            };
            switchCompassEnabled.IsToggled = map.UiSettings.CompassEnabled;

            // RotateGesturesEnabled
            switchRotateGesturesEnabled.Toggled += (sender, e) =>
            {
                map.UiSettings.RotateGesturesEnabled = e.Value;
            };
            switchRotateGesturesEnabled.IsToggled = map.UiSettings.RotateGesturesEnabled;

            // MyLocationButtonEnabled
            switchMyLocationButtonEnabled.Toggled += (sender, e) =>
            {
                map.UiSettings.MyLocationButtonEnabled = e.Value;
            };
            switchMyLocationButtonEnabled.IsToggled = map.UiSettings.MyLocationButtonEnabled;

            // IndoorLevelPickerEnabled
            switchIndoorLevelPickerEnabled.Toggled += (sender, e) =>
            {
                map.UiSettings.IndoorLevelPickerEnabled = e.Value;
            };
            switchIndoorLevelPickerEnabled.IsToggled = map.UiSettings.IndoorLevelPickerEnabled;

            // ScrollGesturesEnabled
            switchScrollGesturesEnabled.Toggled += (sender, e) =>
            {
                map.UiSettings.ScrollGesturesEnabled = e.Value;
            };
            switchScrollGesturesEnabled.IsToggled = map.UiSettings.ScrollGesturesEnabled;

            // TiltGesturesEnabled
            switchTiltGesturesEnabled.Toggled += (sender, e) =>
            {
                map.UiSettings.TiltGesturesEnabled = e.Value;
            };
            switchTiltGesturesEnabled.IsToggled = map.UiSettings.TiltGesturesEnabled;

            // ZoomControlsEnabled
            switchZoomControlsEnabled.Toggled += (sender, e) =>
            {
                map.UiSettings.ZoomControlsEnabled = e.Value;
            };
            switchZoomControlsEnabled.IsToggled = map.UiSettings.ZoomControlsEnabled;

            // ZoomGesturesEnabled
            switchZoomGesturesEnabled.Toggled += (sender, e) =>
            {
                map.UiSettings.ZoomGesturesEnabled = e.Value;
            };
            switchZoomGesturesEnabled.IsToggled = map.UiSettings.ZoomGesturesEnabled;

            // Map Clicked
            map.MapClicked += (sender, e) =>
            {
                var lat = e.Point.Latitude.ToString("0.000");
                var lng = e.Point.Longitude.ToString("0.000");
                this.DisplayAlert("MapClicked", $"{lat}/{lng}", "CLOSE");
            };

            // Map Long clicked
            map.MapLongClicked += (sender, e) =>
            {
                var lat = e.Point.Latitude.ToString("0.000");
                var lng = e.Point.Longitude.ToString("0.000");
                this.DisplayAlert("MapLongClicked", $"{lat}/{lng}", "CLOSE");
            };

            // Map MyLocationButton clicked
            map.MyLocationButtonClicked += (sender, args) =>
            {
                args.Handled = switchHandleMyLocationButton.IsToggled;
                if (switchHandleMyLocationButton.IsToggled)
                {
                    this.DisplayAlert("MyLocationButtonClicked", 
                                 "If set MyLocationButtonClickedEventArgs.Handled = true then skip obtain current location", 
                                 "OK");
                }

            };

            map.CameraChanged += (sender, args) =>
            {
                var p = args.Position;
                labelStatus.Text = $"Lat={p.Target.Latitude:0.00}, Long={p.Target.Longitude:0.00}, Zoom={p.Zoom:0.00}, Bearing={p.Bearing:0.00}, Tilt={p.Tilt:0.00}";
            };

            // Geocode
            buttonGeocode.Clicked += async (sender, e) =>
            {
                var geocoder = new Xamarin.Forms.GoogleMaps.Geocoder();
                var positions = await geocoder.GetPositionsForAddressAsync(entryAddress.Text);
                if (positions.Count() > 0)
                {
                    var pos = positions.First();
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(pos, Distance.FromMeters(5000)));
                    var reg = map.VisibleRegion;
                    var format = "0.00";
                    labelStatus.Text = $"Center = {reg.Center.Latitude.ToString(format)}, {reg.Center.Longitude.ToString(format)}";
                }
                else
                {
                    await this.DisplayAlert("Not found", "Geocoder returns no results", "Close");
                }
            };

            // Snapshot
            buttonTakeSnapshot.Clicked += async (sender, e) =>
            {
                var stream = await map.TakeSnapshot();
                imageSnapshot.Source = ImageSource.FromStream(() => stream);
            };
        }
    }
}

