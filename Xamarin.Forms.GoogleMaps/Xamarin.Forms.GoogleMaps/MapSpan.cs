﻿using System;
using System.Collections.Generic;
using Xamarin.Forms.GoogleMaps.Internals;

namespace Xamarin.Forms.GoogleMaps
{
    [TypeConverter(typeof(MapSpanTypeConverter))]
    public sealed class MapSpan
    {
        const double EarthRadiusKm = GeoConstants.EarthRadiusKm;
        const double EarthCircumferenceKm = GeoConstants.EarthCircumferenceKm;
        const double MinimumRangeDegrees = 0.001 / EarthCircumferenceKm * 360; // 1 meter

        public MapSpan(Position center, double latitudeDegrees, double longitudeDegrees)
        {
            Center = center;
            LatitudeDegrees = Math.Min(Math.Max(latitudeDegrees, MinimumRangeDegrees), 90.0);
            LongitudeDegrees = Math.Min(Math.Max(longitudeDegrees, MinimumRangeDegrees), 180.0);
        }

        public Position Center { get; }

        public double LatitudeDegrees { get; }

        public double LongitudeDegrees { get; }

        public Distance Radius
        {
            get
            {
                double latKm = LatitudeDegreesToKm(LatitudeDegrees);
                double longKm = LongitudeDegreesToKm(Center, LongitudeDegrees);
                return new Distance(1000 * Math.Min(latKm, longKm) / 2);
            }
        }

        public MapSpan ClampLatitude(double north, double south)
        {
            north = Math.Min(Math.Max(north, 0), 90);
            south = Math.Max(Math.Min(south, 0), -90);
            double lat = Math.Max(Math.Min(Center.Latitude, north), south);
            double maxDLat = Math.Min(north - lat, -south + lat) * 2;
            return new MapSpan(new Position(lat, Center.Longitude), Math.Min(LatitudeDegrees, maxDLat), LongitudeDegrees);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj is MapSpan && Equals((MapSpan)obj);
        }

        public static MapSpan FromCenterAndRadius(Position center, Distance radius)
        {
            return new MapSpan(center, 2 * DistanceToLatitudeDegrees(radius), 2 * DistanceToLongitudeDegrees(center, radius));
        }

        public static MapSpan FromPositions(IEnumerable<Position> positions)
        {
            double minLat = double.MaxValue;
            double minLon = double.MaxValue;
            double maxLat = double.MinValue;
            double maxLon = double.MinValue;

            foreach (var p in positions)
            {
                minLat = Math.Min(minLat, p.Latitude);
                minLon = Math.Min(minLon, p.Longitude);
                maxLat = Math.Max(maxLat, p.Latitude);
                maxLon = Math.Max(maxLon, p.Longitude);
            }

            return new MapSpan(new Position((minLat + maxLat) / 2d, (minLon + maxLon) / 2d), maxLat - minLat, maxLon - minLon);
        }

        public static MapSpan FromBounds(Bounds bounds)
        {
            return new MapSpan(bounds.Center, bounds.HeightDegrees, bounds.WidthDegrees);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Center.GetHashCode();
                hashCode = (hashCode * 397) ^ LongitudeDegrees.GetHashCode();
                hashCode = (hashCode * 397) ^ LatitudeDegrees.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(MapSpan left, MapSpan right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(MapSpan left, MapSpan right)
        {
            return !Equals(left, right);
        }

        public MapSpan WithZoom(double zoomFactor)
        {
            double maxDLat = Math.Min(90 - Center.Latitude, 90 + Center.Latitude) * 2;
            return new MapSpan(Center, Math.Min(LatitudeDegrees / zoomFactor, maxDLat), LongitudeDegrees / zoomFactor);
        }

        static double DistanceToLatitudeDegrees(Distance distance)
        {
            return distance.Kilometers / EarthCircumferenceKm * 360;
        }

        static double DistanceToLongitudeDegrees(Position position, Distance distance)
        {
            double latCircumference = LatitudeCircumferenceKm(position);
            return distance.Kilometers / latCircumference * 360;
        }

        bool Equals(MapSpan other)
        {
            return Center.Equals(other.Center) && LongitudeDegrees.Equals(other.LongitudeDegrees) && LatitudeDegrees.Equals(other.LatitudeDegrees);
        }

        static double LatitudeCircumferenceKm(Position position)
        {
            return EarthCircumferenceKm * Math.Cos(position.Latitude * Math.PI / 180.0);
        }

        static double LatitudeDegreesToKm(double latitudeDegrees)
        {
            return EarthCircumferenceKm * latitudeDegrees / 360;
        }

        static double LongitudeDegreesToKm(Position position, double longitudeDegrees)
        {
            double latCircumference = LatitudeCircumferenceKm(position);
            return latCircumference * longitudeDegrees / 360;
        }

        public override string ToString()
        {
            return ToString(DistanceType.Kilometers);
        }
        public string ToString(DistanceType type)
        {
            return "(" + this.Center.ToString() + ")," + (type == DistanceType.Kilometers ? Radius.Kilometers.ToString("F2").Replace(",", ".") + "km" : type == DistanceType.Meters ? Radius.Meters.ToString("F0") + " meters" : Radius.Miles.ToString("F2").Replace(",", ".") + " miles");
        }
    }
}