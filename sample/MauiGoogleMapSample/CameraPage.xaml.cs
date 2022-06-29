using System.Diagnostics;
using Maui.GoogleMaps;
using Microsoft.Maui.Converters;

namespace MauiGoogleMapSample
{
    public partial class CameraPage : ContentPage
    {
        public CameraPage()
        {
            InitializeComponent();

            var pinMelbourne = new Pin() { Label = "Melbourne", Position = new Position(-37.971237, 144.492697) };
            var pinNewyork = new Pin() { Label = "New york", Position = new Position(40.705311, -74.2581874) };
            var pinLisboa = new Pin() { Label = "Lisboa", Position = new Position(38.7436057, -13.6426275) };
            var pinParis = new Pin() { Label = "Paris", Position = new Position(48.8588377, 2.2775173) };
            var pinTokyo = new Pin() { Label = "Tokyo", Position = new Position(35.7104, 139.8093) };
            map.Pins.Add(pinMelbourne);
            map.Pins.Add(pinNewyork);
            map.Pins.Add(pinLisboa);
            map.Pins.Add(pinParis);
            map.Pins.Add(pinTokyo);

            // CameraChanged is obsolete, please use CameraIdled
            map.CameraChanged += (sender, e) => 
            {
                var p = e.Position;
                var text = $"CameraChanged:Lat={p.Target.Latitude:0.00}, Long={p.Target.Longitude:0.00}, Zoom={p.Zoom:0.00}, Bearing={p.Bearing:0.00}, Tilt={p.Tilt:0.00}";
                labelStatus.Text = text;
                Debug.WriteLine(text);
            };

            map.CameraMoveStarted += (sender, e) => 
            {
                var text = $"CameraMoveStarted:IsGesture={e.IsGesture}";
                labelStatus.Text = text;
                Debug.WriteLine(text);
            };

            map.CameraMoving += (sender, e) =>
            {
                var p = e.Position;
                var text = $"CameraMoving:Lat={p.Target.Latitude:0.00}, Long={p.Target.Longitude:0.00}, Zoom={p.Zoom:0.00}, Bearing={p.Bearing:0.00}, Tilt={p.Tilt:0.00}";
                labelStatus.Text = text;
                Debug.WriteLine(text);
            };

            map.CameraIdled += (sender, e) => 
            {
                var p = e.Position;
                var text = $"CameraIdled:Lat={p.Target.Latitude:0.00}, Long={p.Target.Longitude:0.00}, Zoom={p.Zoom:0.00}, Bearing={p.Bearing:0.00}, Tilt={p.Tilt:0.00}";
                labelStatus.Text = text;
                Debug.WriteLine(text);

                UpdateCornerPins();
            };

            // MoveToCamera with Position
            buttonMoveToPosition.Clicked += async (sender, e) =>
            {
                await map.MoveCamera(CameraUpdateFactory.NewPosition(
                    pinMelbourne.Position)); // Melbourne
            };

            // MoveToCamera with Position and Zoom
            buttonMoveToPositionZoom.Clicked += async (sender, e) =>
            {
                await map.MoveCamera(CameraUpdateFactory.NewPositionZoom(
                    pinNewyork.Position, 16d)); // New york
            };

            // MoveToCamera with Bounds
            buttonMoveToBounds.Clicked += async (sender, e) =>
            {
                await map.MoveCamera(CameraUpdateFactory.NewBounds(
                    new Bounds(pinLisboa.Position,  // Lisboa
                               pinParis.Position),  // Paris
                   50)); // 50px
            };

            // MoveToCamera with CameraPosition
            buttonMoveToCameraPosition.Clicked += async (sender, e) =>
            {
                await map.MoveCamera(CameraUpdateFactory.NewCameraPosition(
                    new CameraPosition(
                        pinTokyo.Position, // Tokyo
                        17d, // zoom
                        45d, // bearing(rotation)
                        60d // tilt
                        )));
            };

            // AnimateToCamera with Position
            buttonAnimateToPosition.Clicked += async (sender, e) =>
            {
                var animState = await map.AnimateCamera(CameraUpdateFactory.NewPosition(
                    pinMelbourne.Position)); // Melbourne
                Debug.WriteLine($"Animate with Position result = {animState}");
            };

            // AnimateToCamera with Position and Zoom
            buttonAnimateToPositionZoom.Clicked += async (sender, e) =>
            {
                var animState = await map.AnimateCamera(CameraUpdateFactory.NewPositionZoom(
                    pinNewyork.Position, 16d), TimeSpan.FromSeconds(1)); // New york
                Debug.WriteLine($"Animate with Position and Zoom result = {animState}");
            };

            // AnimateToCamera with Bounds
            buttonAnimateToBounds.Clicked += async (sender, e) =>
            {
                var animState = await map.AnimateCamera(CameraUpdateFactory.NewBounds(
                    new Bounds(pinLisboa.Position,  // Lisboa
                               pinParis.Position),  // Paris
                   50), TimeSpan.FromSeconds(3)); // 50px
                Debug.WriteLine($"Animate with Bounds result = {animState}");
            };

            // AnimateToCamera with CameraPosition
            buttonAnimateToCameraPosition.Clicked += async (sender, e) =>
            {
                var animState = await map.AnimateCamera(CameraUpdateFactory.NewCameraPosition(
                    new CameraPosition(
                        pinTokyo.Position, // Tokyo
                        17d, // zoom
                        45d, // bearing(rotation)
                        60d)), // tilt
                    TimeSpan.FromSeconds(5));
                Debug.WriteLine($"Animate with CameraPosition result = {animState}");
            };

            // Padding for Map
            entryPadding.TextChanged += (sender, e) =>
            {
                try
                {
                    var converter = new ThicknessTypeConverter();
                    var padding = (Thickness)converter.ConvertFromInvariantString(entryPadding.Text);
                    map.Padding = padding;
                }
                catch (Exception)
                {
                }
            };
            map.Padding = new Thickness(0, 0, 0, 0);
        }

        private void UpdateCornerPins()
        {
            map.Pins.Clear();

            map.Pins.Add(new Pin()
            {
                Label = "FarLeft",
                Position = map.Region.FarLeft,
                Rotation = 135f
            });

            map.Pins.Add(new Pin()
            {
                Label = "FarRight",
                Position = map.Region.FarRight,
                Rotation = -135f
            });

            map.Pins.Add(new Pin()
            {
                Label = "NearLeft",
                Position = map.Region.NearLeft,
                Rotation = 45f
            });

            map.Pins.Add(new Pin()
            {
                Label = "NearRight",
                Position = map.Region.NearRight,
                Rotation = -45f
            });
        }
    }
}
