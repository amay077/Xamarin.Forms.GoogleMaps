﻿using Newtonsoft.Json;
using System;

namespace Xamarin.Forms.GoogleMaps
{
    [JsonConverter(typeof(Internals.PositionJsonConverter))]
    public struct Position
    {
        public Position(double latitude, double longitude)
        {
            Latitude = Math.Min(Math.Max(latitude, -90.0), 90.0);
            Longitude = Math.Min(Math.Max(longitude, -180.0), 180.0);
        }

        public double Latitude { get; }

        public double Longitude { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (!(obj is Position) || ReferenceEquals(null, this))
                return false;
            var other = (Position)obj;
            return Latitude == other.Latitude && Longitude == other.Longitude;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Latitude.GetHashCode();
                hashCode = (hashCode * 397) ^ Longitude.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Position left, Position right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !Equals(left, right);
        }

        public static Position operator -(Position left, Position right)
        {
            return new Position(left.Latitude - right.Latitude, left.Longitude - right.Longitude);
        }

        public override string ToString()
        {
            return Latitude.ToString("F5").Replace(",", ".") + "," + Longitude.ToString("F5").Replace(",", ".");
        }
    }
}