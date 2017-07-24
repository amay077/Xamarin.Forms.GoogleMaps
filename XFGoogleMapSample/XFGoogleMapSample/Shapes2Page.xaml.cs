using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
    public partial class Shapes2Page : ContentPage
    {
        public Shapes2Page()
        {
            InitializeComponent();

            pickerZIndex.Items.Add("Red");
            pickerZIndex.Items.Add("Yellow");
            pickerZIndex.Items.Add("Green");

            // Add Polygons
            var polygon1 = new Polygon();
            polygon1.Positions.Add(new Position(35.65, 139.83));
            polygon1.Positions.Add(new Position(35.75, 139.93));
            polygon1.Positions.Add(new Position(35.85, 139.83));
            polygon1.Positions.Add(new Position(35.65, 139.83));
            polygon1.StrokeWidth = 1f;
            polygon1.StrokeColor = Color.Red;
            polygon1.FillColor = Color.Red;
            map.Polygons.Add(polygon1);
            map.Polygons.Add(CreateShiftedPolygon(polygon1, 0d, 0.05d, Color.Yellow));
            map.Polygons.Add(CreateShiftedPolygon(polygon1, 0d, 0.10d, Color.Green));

            // Add Polylines
            var polyline1 = new Polyline();
            polyline1.StrokeWidth = 10f;
            polyline1.StrokeColor = Color.Red;
            polyline1.Positions.Add(new Position(36.00, 139.83));
            polyline1.Positions.Add(new Position(36.10, 139.93));
            polyline1.Positions.Add(new Position(36.00, 140.03));
            map.Polylines.Add(polyline1);
            map.Polylines.Add(CreateShiftedPolyline(polyline1, 0d, 0.05d, Color.Yellow));
            map.Polylines.Add(CreateShiftedPolyline(polyline1, 0d, 0.10d, Color.Green));

            // Add Circles
            var circle1 = new Circle();
            circle1.StrokeWidth = 10f;
            circle1.StrokeColor = Color.Red;
            circle1.FillColor = Color.Red;
            circle1.Center = new Position(35.85, 140.23);
            circle1.Radius = Distance.FromKilometers(8);
            map.Circles.Add(circle1);
            map.Circles.Add(CreateShiftedCircle(circle1, 0d, 0.05d, Color.Yellow));
            map.Circles.Add(CreateShiftedCircle(circle1, 0d, 0.10d, Color.Green));

            // Fit to all shapes
            var bounds = Xamarin.Forms.GoogleMaps.Bounds.FromPositions(map.Polygons.SelectMany(poly => poly.Positions));
            bounds = bounds.Including(Xamarin.Forms.GoogleMaps.Bounds.FromPositions(map.Polylines.SelectMany(poly => poly.Positions)));
            bounds = bounds.Including(Xamarin.Forms.GoogleMaps.Bounds.FromPositions(map.Circles.Select(cir => cir.Center)));

            // Move to Front
            pickerZIndex.SelectedIndexChanged += (sender, e) =>
            {
                var i = pickerZIndex.SelectedIndex;
                map.Polylines[i].ZIndex = map.Polylines.Max(p => p.ZIndex) + 1;
                map.Polygons[i].ZIndex = map.Polygons.Max(p => p.ZIndex) + 1;
                map.Circles[i].ZIndex = map.Circles.Max(p => p.ZIndex) + 1;
            };
            pickerZIndex.SelectedIndex = 0;

            map.InitialCameraUpdate = CameraUpdateFactory.NewBounds(bounds, 5);
        }

        private Polygon CreateShiftedPolygon(Polygon polygon, double shiftLat, double shiftLon, Color color)
        {
            var poly = new Polygon();
            poly.StrokeWidth = polygon.StrokeWidth;
            poly.StrokeColor = color;
            poly.FillColor = color;

            foreach (var p in polygon.Positions)
            {
                poly.Positions.Add(new Position(p.Latitude + shiftLat, p.Longitude + shiftLon));
            }

            return poly;
        }

        private Polyline CreateShiftedPolyline(Polyline polyline, double shiftLat, double shiftLon, Color color)
        {
            var poly = new Polyline();
            poly.StrokeWidth = polyline.StrokeWidth;
            poly.StrokeColor = color;

            foreach (var p in polyline.Positions)
            {
                poly.Positions.Add(new Position(p.Latitude + shiftLat, p.Longitude + shiftLon));
            }

            return poly;
        }

        private Circle CreateShiftedCircle(Circle circle, double shiftLat, double shiftLon, Color color)
        {
            var cir = new Circle();
            cir.StrokeWidth = circle.StrokeWidth;
            cir.StrokeColor = color;
            cir.FillColor = color;
            cir.Radius = circle.Radius;
            cir.Center = new Position(circle.Center.Latitude + shiftLat, circle.Center.Longitude + shiftLon);

            return cir;
        }


    }
}
