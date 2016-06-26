using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    public partial class ShapesPage : ContentPage
    {
        public ShapesPage()
        {
            InitializeComponent();

            Polyline polyline = null;
            Polygon polygon = null;
            Circle circle = null;

            // Polyline
            buttonAddPolyline.Clicked += (sender, e) => 
            {
                polyline = new Polyline();
                polyline.Positions.Add(new Position(40.77d, -73.93d));
                polyline.Positions.Add(new Position(40.81d, -73.91d));
                polyline.Positions.Add(new Position(40.83d, -73.87d));

                polyline.IsClickable = true;
                polyline.StrokeColor = Color.Blue;
                polyline.StrokeWidth = 5f;
                polyline.Tag = "POLYLINE"; // Can set any object

                polyline.Clicked += Polyline_Clicked;

                map.Polylines.Add(polyline);

                ((Button)sender).IsEnabled = false;
                buttonRemovePolyline.IsEnabled = true;
            };

            buttonRemovePolyline.Clicked += (sender, e) => 
            {
                map.Polylines.Remove(polyline);
                polyline.Clicked -= Polyline_Clicked;
                polyline = null;

                ((Button)sender).IsEnabled = false;
                buttonAddPolyline.IsEnabled = true;
            };
            buttonRemovePolyline.IsEnabled = false;

            // Polygon
            buttonAddPolygon.Clicked += (sender, e) =>
            {
                polygon = new Polygon();
                polygon.Positions.Add(new Position(40.85d, -73.96d));
                polygon.Positions.Add(new Position(40.87d, -74.00d));
                polygon.Positions.Add(new Position(40.78d, -74.06d));
                polygon.Positions.Add(new Position(40.77d, -74.02d));

                polygon.IsClickable = true;
                polygon.StrokeColor = Color.Green;
                polygon.StrokeWidth = 3f;
                polygon.FillColor = Color.FromRgba(255, 0, 0, 64);
                polygon.Tag = "POLYGON"; // Can set any object

                polygon.Clicked += Polygon_Clicked;;

                map.Polygons.Add(polygon);

                ((Button)sender).IsEnabled = false;
                buttonRemovePolygon.IsEnabled = true;
            };

            buttonRemovePolygon.Clicked += (sender, e) =>
            {
                map.Polygons.Remove(polygon);
                polygon.Clicked -= Polygon_Clicked;
                polygon = null;

                ((Button)sender).IsEnabled = false;
                buttonAddPolygon.IsEnabled = true;
            };
            buttonRemovePolygon.IsEnabled = false;

            // Circle
            buttonAddCircle.Clicked += (sender, e) =>
            {
                circle = new Circle();
                circle.Center = new Position(40.72d, -73.89d);
                circle.Radius = Distance.FromMeters(3000f);

                circle.StrokeColor = Color.Purple;
                circle.StrokeWidth = 6f;
                circle.FillColor = Color.FromRgba(0, 0, 255, 32);
                circle.Tag = "CIRCLE"; // Can set any object

                map.Circles.Add(circle);

                ((Button)sender).IsEnabled = false;
                buttonRemoveCircle.IsEnabled = true;
            };

            buttonRemoveCircle.Clicked += (sender, e) =>
            {
                map.Circles.Remove(circle);
                circle = null;

                ((Button)sender).IsEnabled = false;
                buttonAddCircle.IsEnabled = true;
            };
            buttonRemoveCircle.IsEnabled = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(40.78d, -73.96d), Distance.FromMeters(10000)), false);
        }

        void Polyline_Clicked(object sender, EventArgs e)
        {
            var polyline = (Polyline)sender;
            DisplayAlert("Polyline", $"{(string)polyline.Tag} Clicked!", "Close");
        }

        void Polygon_Clicked(object sender, EventArgs e)
        {
            var polygon = (Polygon)sender;
            DisplayAlert("Polygon", $"{(string)polygon.Tag} Clicked!", "Close");
        }
    }
}

