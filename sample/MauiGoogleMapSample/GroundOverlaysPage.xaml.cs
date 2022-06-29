using System.Reflection;
using Maui.GoogleMaps;

namespace MauiGoogleMapSample
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
            sliderTransparency.Value = 3;
            sliderTransparency.ValueChanged += (sender, e) =>
            {
                _overlay.Transparency = (float)(e.NewValue / 10f);
            };

            // Bearing
            sliderBearing.Value = 0;
            sliderBearing.ValueChanged += (sender, e) =>
            {
                _overlay.Bearing = (float)e.NewValue;
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
                Icon = BitmapDescriptorFactory.FromBundle("image01"),
                Tag = "THE GROUNDOVERLAY",
                Transparency = (float)(sliderTransparency.Value / 10f),
                ZIndex = 2
            };
            map.GroundOverlays.Add(_overlay);

            var offset = 0.00025d;
            map.GroundOverlays.Add(new GroundOverlay()
            {
                Bounds = new Bounds(
                    new Position(37.797496 + offset, -122.402054 + offset), 
                    new Position(37.798573 + offset, -122.401065 + offset)),
                Icon = BitmapDescriptorFactory.FromBundle("image02"),
                ZIndex = 1
            });

            // ZIndex(to Front) 
            buttonToFront.Clicked += (sender, e) =>
            {
                _overlay.ZIndex = 2;
            };

            // ZIndex(to Back) 
            buttonToBack.Clicked += (sender, e) =>
            {
                _overlay.ZIndex = 0;
            };

            _overlay.Clicked += (sender, e) =>
            {
                var overlay = sender as GroundOverlay;
                this.DisplayAlert("Clicked", overlay.Tag as string, "CLOSE");
            };

            var polygon = new Polygon()
            {
                FillColor = Colors.Transparent,
                StrokeColor = Colors.Blue,
                StrokeWidth = 2f
            };
            polygon.Positions.Add(new Position(37.797496, -122.402054));
            polygon.Positions.Add(new Position(37.797496, -122.401065));
            polygon.Positions.Add(new Position(37.798573, -122.401065));
            polygon.Positions.Add(new Position(37.798573, -122.402054));
            polygon.Positions.Add(new Position(37.797496, -122.402054));

            map.Polygons.Add(polygon);

            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(37.797496 + offset, -122.402054 + offset), Distance.FromMeters(200)), false);
        }

        void UpdateIcon(int selectedIndex)
        {
            switch (selectedIndex)
            {
                case 0: // Bundle
                    _overlay.Icon = BitmapDescriptorFactory.FromBundle("image01");
                    break;
                case 1: // Stream
                    var assembly = typeof(GroundOverlaysPage).GetTypeInfo().Assembly;
                    var stream = assembly.GetManifestResourceStream($"MauiGoogleMapSample.marker01.png");
                    _overlay.Icon = BitmapDescriptorFactory.FromStream(stream, id: "1"); // id is used for caching purposes
                    break;
                case 2: // DefaultMarker
                    _overlay.Icon = BitmapDescriptorFactory.DefaultMarker(Colors.Blue);
                    break;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();


        }
    }
}

