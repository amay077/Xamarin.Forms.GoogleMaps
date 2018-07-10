using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using Xamarin.Forms.Xaml;

namespace XFGoogleMapSample.local
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BoundsTestPage : ContentPage
    {
        public BoundsTestPage ()
        {
            InitializeComponent ();

            // Colosseum of Rome: 41.890251, 12.492373
            var center = new Position(41.890251, 12.492373);

            //mainMap.MoveToRegion(new MapSpan(center, 0.2, 0.2), false);
            mainMap.UiSettings.RotateGesturesEnabled = false;
            mainMap.UiSettings.ZoomControlsEnabled = false;
            mainMap.UiSettings.CompassEnabled = false;
            mainMap.CameraIdled += MainMap_CameraIdled;
            mainMap.CameraMoveStarted += MainMap_CameraMoveStarted;
            mainMap.MapClicked += MainMap_MapClicked;
        }

        private void MainMap_MapClicked(object sender, MapClickedEventArgs e)
        {
            Map _m = sender as Map;
            Position clickPoint = e.Point;

            // Now calculate the minimum value to accept point as "clicked", based on current Visible Area...
            var currentAreaRadius = _m.VisibleRegion.Radius.Kilometers;
            double acceptableValue = currentAreaRadius * 0.15;

            Debug.WriteLine($"Map clicked at position {e.Point.Latitude} - {e.Point.Longitude}");
        }

        private void MainMap_CameraMoveStarted(object sender, CameraMoveStartedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void MainMap_CameraIdled(object sender, CameraIdledEventArgs e)
        {
            //Map _m = sender as Map;
            //var region = _m.VisibleRegion;
            //var center = region.Center;
            //var halfheightDegrees = region.LatitudeDegrees / 2;
            //var halfwidthDegrees = region.LongitudeDegrees / 2;

            //var left = center.Longitude - halfwidthDegrees;
            //var right = center.Longitude + halfwidthDegrees;
            //var top = center.Latitude + halfheightDegrees;
            //var bottom = center.Latitude - halfheightDegrees;

            //// Adjust for Internation Date Line (+/- 180 degrees longitude)
            //if (left < -180) left = 180 + (180 + left);
            //if (right > 180) right = (right - 180) - 180;

            Map _map = sender as Map;
            //Bounds bounds = Xamarin.Forms.GoogleMaps.Extensions.MapSpanExtensions.ToBounds(_map.VisibleRegion);
            //Debug.WriteLine($"[Map bounds]\r\nNW: {bounds.NorthWest.Latitude}, {bounds.NorthWest.Longitude}\r\nNE: {bounds.NorthEast.Latitude}, {bounds.NorthEast.Longitude}\r\n" +
            //    $"SW: {bounds.SouthWest.Latitude}, {bounds.SouthWest.Longitude}\r\nSE: {bounds.SouthEast.Latitude}, {bounds.SouthEast.Longitude}");

            //Debug.WriteLine($"[_map bound]\r\nTL: {_map.TopLeft.Latitude}, {_map.TopLeft.Longitude}\r\nTR: {_map.TopRight.Latitude}, {_map.TopRight.Longitude}\r\n" +
            //    $"BL: {_map.BottomLeft.Latitude}, {_map.BottomLeft.Longitude}\r\nBR: {_map.BottomRight.Latitude}, {_map.BottomRight.Longitude}");

            if (_map.Pins.Count > 0)
            {
                _map.Pins.Clear();
            }
            _map.Pins.Add(new Pin()
            {
                Label = "TL",
                Position = _map.TopLeft,
                Rotation = 135f
            });

            _map.Pins.Add(new Pin()
            {
                Label = "TR",
                Position = _map.TopRight,
                Rotation = -135f
            });

            _map.Pins.Add(new Pin()
            {
                Label = "BL",
                Position = _map.BottomLeft,
                Rotation = 45f
            });

            _map.Pins.Add(new Pin()
            {
                Label = "BR",
                Position = _map.BottomRight,
                Rotation = -45f
            });

            // Now I should convert coordinates to map...
            Position Rome = new Position(41.890251, 12.492373);
            _map.Pins.Add(new Pin()
            {
                Label = "Rome real",
                Position = Rome,
                Rotation = 15f,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.Blue)
            });

            double yMerTl = MercatorProjection.latToY(_map.TopLeft.Latitude);
            double xMerTl = MercatorProjection.lonToX(_map.TopLeft.Longitude);

            double yMerBr = MercatorProjection.latToY(_map.BottomRight.Latitude);
            double xMerBr = MercatorProjection.lonToX(_map.BottomRight.Longitude);

            double xRome = MercatorProjection.lonToX(Rome.Longitude);
            double yRome = MercatorProjection.latToY(Rome.Latitude);

            Position romeFromMer = new Position(MercatorProjection.yToLat(yRome), MercatorProjection.xToLon(xRome));
            _map.Pins.Add(new Pin()
            {
                Label = "Rome mercator",
                Position = romeFromMer,
                Rotation = -15f,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.DarkGreen)
            });

            double xScale = (double)_map.Width / Math.Abs(xMerBr - xMerTl);
            double yScale = -(double)_map.Height / Math.Abs(yMerTl - yMerBr);

            // get rome x and y respect to width / height...
            double xRelativeRome = (xRome - xMerTl) * xScale;
            double yRelativeRome = (yRome - yMerTl) * yScale;

            double xInverseRome = (xRelativeRome / xScale) + xMerTl;
            double yInverseRome = (yRelativeRome / yScale) + yMerTl;
            Position romeInverse = new Position(MercatorProjection.yToLat(yInverseRome), MercatorProjection.xToLon(xRome));
            _map.Pins.Add(new Pin()
            {
                Label = "Rome inverse",
                Position = romeInverse,
                Rotation = -35f,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.AliceBlue)
            });

            // Now place always a point on the center of map, using scaled x-y values...
            Point centerPoint = new Point(_map.Width / 2, _map.Height / 2);
            double xCenter = (centerPoint.X / xScale) + xMerTl;
            double yCenter = (centerPoint.Y / yScale) + yMerTl;
            // TODO: check Longitude result...
            // Adjust for Internation Date Line (+/- 180 degrees longitude)
            var xLon = MercatorProjection.xToLon(xCenter);
            if (xLon < -180)
            {
                xLon = 180 + (180 + xLon);
            }
            if (xLon > 180)
            {
                xLon = (xLon - 180) - 180;
            }

            var yLat = MercatorProjection.yToLat(yCenter);

            Position centerPos = new Position(yLat, xLon);//MercatorProjection.yToLat(yCenter), MercatorProjection.xToLon(xCenter));
            _map.Pins.Add(new Pin()
            {
                Label = "Center inverse",
                Position = centerPos,
                Rotation = -180f,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.Yellow)
            });

            // Add tests limits points:
            _map.Pins.Add(new Pin()
            {
                Label = "0,0",
                Position = new Position(0d,0d),
                Rotation = 0f,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.AliceBlue)
            });

            _map.Pins.Add(new Pin()
            {
                Label = "0,180",
                Position = new Position(0d, 180d),
                Rotation = 0f,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.WhiteSmoke)
            });
            _map.Pins.Add(new Pin()
            {
                Label = "0,-180",
                Position = new Position(0d, 180d),
                Rotation = -80f,
                Icon = BitmapDescriptorFactory.DefaultMarker(Color.Violet)
            });
        }
    }
}