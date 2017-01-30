
using System;

namespace Xamarin.Forms.GoogleMaps
{
    public class Bounds
    {
        public Position SouthWest { get; }
        public Position NorthEast { get; }
        public Position SouthEast
        {
            get
            {
                return new Position(SouthWest.Latitude, NorthEast.Longitude);
            }
        }

        public Position NorthWest {
            get
            {
                return new Position(NorthEast.Latitude, SouthWest.Longitude);
            }
        }

        public Position Center
        {
            get
            {
                return new Position((SouthWest.Latitude + NorthEast.Latitude) / 2d,
                              (SouthWest.Longitude + NorthEast.Longitude) / 2d);
            }
        }

        public double WidthDegrees
        {
            get
            {
                return Math.Abs(NorthEast.Longitude - SouthWest.Longitude);
            }
        }

        public double HeightDegrees
        {
            get
            {
                return Math.Abs(NorthEast.Latitude - SouthWest.Latitude);
            }
        }

        public Bounds(Position southWest, Position northEast)
        {
            SouthWest = southWest;
            NorthEast = northEast;
        }

        public Bounds Including(Position position)
        {
            var minX = Math.Min(SouthWest.Longitude, position.Longitude);
            var minY = Math.Min(SouthWest.Latitude, position.Latitude);
            var maxX = Math.Max(NorthEast.Longitude, position.Longitude);
            var maxY = Math.Max(NorthEast.Latitude, position.Latitude);

            return new Bounds(new Position(minY, minX), new Position(maxY, maxX));
        }

        public bool Contains(Position position)
        {
            return SouthWest.Longitude <= position.Longitude && position.Longitude <= NorthEast.Longitude
                    && SouthWest.Latitude <= position.Latitude && position.Latitude <= NorthEast.Latitude;
        }
    }
}


