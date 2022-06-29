using Maui.GoogleMaps;

namespace MauiGoogleMapSample
{
    public partial class ShapesWithInitializePage : ContentPage
    {
        public ShapesWithInitializePage()
        {
            InitializeComponent();

            var polyline = new Polyline();
            polyline.Positions.Add(new Position(40.77d, -73.93d));
            polyline.Positions.Add(new Position(40.81d, -73.91d));
            polyline.Positions.Add(new Position(40.83d, -73.87d));
            polyline.IsClickable = true;
            polyline.StrokeColor = Colors.Blue;
            polyline.StrokeWidth = 5f;
            map.Polylines.Add(polyline);

            var polygon = new Polygon();
            polygon.Positions.Add(new Position(40.85d, -73.96d));
            polygon.Positions.Add(new Position(40.87d, -74.00d));
            polygon.Positions.Add(new Position(40.78d, -74.06d));
            polygon.Positions.Add(new Position(40.77d, -74.02d));
            polygon.IsClickable = true;
            polygon.StrokeColor = Colors.Green;
            polygon.StrokeWidth = 3f;
            polygon.FillColor = Color.FromRgba(255, 0, 0, 64);
            map.Polygons.Add(polygon);

            var circle = new Circle();
            circle.Center = new Position(40.72d, -73.89d);
            circle.Radius = Distance.FromMeters(3000f);
            circle.StrokeColor = Colors.Purple;
            circle.StrokeWidth = 6f;
            circle.FillColor = Color.FromRgba(0, 0, 255, 32);
            map.Circles.Add(circle);

            var pinNewYork = new Pin()
            {
                Type = PinType.Place,
                Label = "Central Park NYC",
                Address = "New York City, NY 10022",
                Position = new Position(40.78d, -73.96d),
                IsDraggable = true
            };
            map.Pins.Add(pinNewYork);
            map.SelectedPin = pinNewYork;

            map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(40.78d, -73.96d), Distance.FromMeters(10000)), false);
        }
    }
}

