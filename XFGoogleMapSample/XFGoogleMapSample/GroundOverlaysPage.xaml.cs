using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    public partial class GroundOverlaysPage : ContentPage
    {
        public GroundOverlaysPage()
        {
            InitializeComponent();

            var overlay = new GroundOverlay()
            {
                Bounds = new Bounds(new Position(37.797496, -122.402054), new Position(37.798573, -122.401065)),
                Icon = BitmapDescriptorFactory.FromBundle("image02.png"),
                Transparency = .5f
            };
            map.GroundOverlays.Add(overlay);

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

        protected override void OnAppearing()
        {
            base.OnAppearing();


        }
    }
}

