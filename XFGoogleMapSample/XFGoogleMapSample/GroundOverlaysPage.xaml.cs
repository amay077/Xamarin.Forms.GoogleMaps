using System;
using System.Collections.Generic;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    public partial class GroundOverlaysPage : ContentPage
    {
        readonly string[] _icons =
        {
            "Bundle",
            "Stream",
            "DefaultMarker"
        };

        readonly GroundOverlay _overlay;

        public GroundOverlaysPage()
        {
            InitializeComponent();

            // Icon
            pickerIcon.Items.AddAll(_icons);
            pickerIcon.SelectedIndex = 0;
            pickerIcon.SelectedIndexChanged += (sender, e) =>
            {
                UpdateIcon(pickerIcon.SelectedIndex);
            };

            // Transparency
            entryTransparency.Text = "0.5";
            entryTransparency.TextChanged += (sender, e) => 
            {
                var transparency = 0f;
                if (float.TryParse(e.NewTextValue, out transparency))
                {
                    _overlay.Transparency = transparency;
                }
            };

            // Bearing
            entryBearing.Text = "0";
            entryBearing.TextChanged += (sender, e) =>
            {
                var bearing = 0f;
                if (float.TryParse(e.NewTextValue, out bearing))
                {
                    _overlay.Bearing = bearing;
                }
            };

            // IsClickable
            switchIsClickable.IsToggled = false;
            switchIsClickable.Toggled += (sender, e) => 
            {
                _overlay.IsClickable = e.Value;
            };

            _overlay = new GroundOverlay()
            {
                Bounds = new Bounds(new Position(37.797496, -122.402054), new Position(37.798573, -122.401065)),
                Icon = BitmapDescriptorFactory.FromBundle("image01.png"),
                Transparency = 0.5f,
                Tag = "THE GROUNDOVERLAY"
            };
            map.GroundOverlays.Add(_overlay);

            _overlay.Clicked += (sender, e) => 
            {
                var overlay = sender as GroundOverlay;
                this.DisplayAlert("Clicked", overlay.Tag as string, "CLOSE");
            };


            var polygon = new Polygon()
            {
                FillColor = Color.Transparent,
                StrokeColor = Color.Blue,
                StrokeWidth = 2f
            };
            polygon.Positions.Add(new Position(37.797496, -122.402054));
            polygon.Positions.Add(new Position(37.797496, -122.401065));
            polygon.Positions.Add(new Position(37.798573, -122.401065));
            polygon.Positions.Add(new Position(37.798573, -122.402054));
            polygon.Positions.Add(new Position(37.797496, -122.402054));

            map.Polygons.Add(polygon);

            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(37.797496, -122.402054), Distance.FromMeters(200)), false);
        }

        void UpdateIcon(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0: // Bundle
                    _overlay.Icon = BitmapDescriptorFactory.FromBundle("image01.png");
                    break;
                case 1: // Stream
                    var assembly = typeof(GroundOverlaysPage).GetTypeInfo().Assembly;
                    var stream = assembly.GetManifestResourceStream($"XFGoogleMapSample.marker01.png");
                    _overlay.Icon = BitmapDescriptorFactory.FromStream(stream);
                    break;
                case 2: // DefaultMarker
                    _overlay.Icon = BitmapDescriptorFactory.DefaultMarker(Color.Blue);
                    break;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();


        }
    }
}

