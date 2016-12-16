using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps
{
    public static class GeoExtensions
    {
        const double EarthRadiusKm = GeoConstants.EarthRadiusKm;

        public const double ToRadiansFactorD = 1.0d / 180d * (double)Math.PI;

        public static double ToRadians(this double val)
            => ToRadiansFactorD * val;

        public static double ToDegrees(this double val)
            => val / ToRadiansFactorD;

        public static bool IsNullOrEmpty(Position ll) { if (ll == null) return true; if (ll.Latitude == 0 || ll.Longitude == 0) return true; return false; }

        #region Exact distance calculations
        public static double MilesTo(this Position gp, Position gp2)
            => KmTo(gp, gp2) * GeoConstants.MetersPerKilometer / GeoConstants.MetersPerMile;

        public static double MetersTo(this Position gp, Position gp2)
            => KmTo(gp, gp2) * GeoConstants.MetersPerKilometer;

        public static double KmTo_Degrees(double lat1Degrees, double lng1Degrees, double lat2Degrees, double lng2Degrees)
            => KmTo(lat1Degrees.ToRadians(), lng1Degrees.ToRadians(), lat2Degrees.ToRadians(), lng2Degrees.ToRadians());

        public static double KmTo(double lat1Rad, double lng1Rad, double lat2Rad, double lng2Rad)
        {
            var demiDLat = (lat2Rad - lat1Rad) / 2;
            var demiDLon = (lng2Rad - lng1Rad) / 2;

            var a =
                Math.Pow(Math.Sin(demiDLat), 2) +  //    Sin²(a)=(1-cos(2a))/2
                                                   //(1 - Math.Cos(2 * demiDLat)) / 2 +
                Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                Math.Pow(Math.Sin(demiDLon), 2);
                                                   //(1 - Math.Cos(2 * demiDLon)) / 2;
            var c = Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return c * 2 * EarthRadiusKm;
        }

        public static double KmTo(this Position gp, Position gp2)
            => (gp2 == null) ? double.MaxValue : KmTo_Degrees(gp.Latitude, gp.Longitude, gp2.Latitude, gp2.Longitude);

        public static double KmTo(this Position gp, double lat2Degrees, double lng2Degrees)
            => KmTo_Degrees(gp.Latitude, gp.Longitude, lat2Degrees, lng2Degrees);

        public static double KmTo(this Pin pin, Pin pin2)
            => pin.Position.KmTo(pin2.Position);

        public static double KmTo(this IPin pin, IPin pin2)
            => pin.PinPosition.KmTo(pin2.PinPosition);

        public static double KmTo(this Pin pin, Position pos)
            => pin.Position.KmTo(pos);

        public static double KmTo(this IPin pin, Position pos)
            => pin.PinPosition.KmTo(pos);

        #endregion Exact distance calculations

        #region Approximate distance calculations
        /// <summary>
        /// Provides an approximation of the distance between two points based on an equirectangular projection
        /// Calculation much faster than the exact distance, to sort positions by distance for example
        /// </summary>
        /// <see cref="http://www.movable-type.co.uk/scripts/latlong.html"/>
        /// <param name="lat1Rad"></param>
        /// <param name="lng1Rad"></param>
        /// <param name="lat2Rad"></param>
        /// <param name="lng2Rad"></param>
        /// <returns></returns>
        public static double EquirectangularKmTo(double lat1Rad, double lng1Rad, double lat2Rad, double lng2Rad)
        {
            // http://www.movable-type.co.uk/scripts/latlong.html
            var x = (lng2Rad - lng1Rad) * Math.Cos((lat1Rad + lat2Rad) / 2);
            var y = (lat2Rad - lat1Rad);
            return Math.Sqrt(x * x + y * y) * GeoConstants.EarthRadiusKm;
        }

        public static double EquirectangularKmTo_Degrees(double lat1Deg, double lng1Deg, double lat2Deg, double lng2Deg)
        {
            // http://www.movable-type.co.uk/scripts/latlong.html
            var x = (lng2Deg - lng1Deg) * ToRadiansFactorD * Math.Cos((lat1Deg + lat2Deg) * ToRadiansFactorD / 2);
            var y = (lat2Deg - lat1Deg) * ToRadiansFactorD;
            return Math.Sqrt(x * x + y * y) * GeoConstants.EarthRadiusKm;
        }

        public static double EquirectangularKmTo(this Position p1, Position p2)
            => EquirectangularKmTo_Degrees(p1.Latitude, p1.Longitude, p2.Latitude, p2.Longitude);


        /// <summary>
        /// A fast distance-like value that can be used to order positions by distance.
        /// Not compatible with <seealso cref="DistanceToOrder_Degrees(double, double, double, double)"/>
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double Distance2ToOrder(double lat1Rad, double lng1Rad, double lat2Rad, double lng2Rad)
        {
            var x = (lng2Rad - lng1Rad) * Math.Cos((lat1Rad + lat2Rad) / 2);
            var y = (lat2Rad - lat1Rad);
            return x * x + y * y;
        }

        /// <summary>
        /// Another fast distance-like value that can be used to order positions by distance.
        /// Not compatible with <seealso cref="DistanceToOrder(double, double, double, double)"/>
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lng1"></param>
        /// <param name="lat2"></param>
        /// <param name="lng2"></param>
        /// <returns></returns>
        public static double DistanceToOrder_Degrees(double lat1Deg, double lng1Deg, double lat2Deg, double lng2Deg)
        {
            var x = (lng2Deg - lng1Deg) * Math.Cos((lat1Deg + lat2Deg) * ToRadiansFactorD / 2);
            var y = (lat2Deg - lat1Deg);
            return x * x + y * y;
        }

        public static double DistanceToOrder(this Position p1, Position p2)
            => DistanceToOrder_Degrees(p1.Latitude, p1.Longitude, p2.Latitude, p2.Longitude);

        #endregion Approximate distance calculations

        #region Position & Circle stories

        public static bool IsIn(this Position pos, ICircle c)
            => pos.KmTo(c.CircleCenter) <= c.CircleRadius.Kilometers;
        public static bool IsIn(this Position pos, Circle c)
            => pos.KmTo(c.Center) <= c.Radius.Kilometers;

        public static bool Contains(this ICircle c, Position pos)
            => IsIn(pos, c);
        public static bool Contains(this Circle c, Position pos)
            => IsIn(pos, c);

        #endregion Position & Circle stories

        public static bool Intersects(this ISegment s, ICircle c)
            => s.Position1.IsIn(c) ^ s.Position2.IsIn(c);

        #region Position translations

        public static Position TranslateToBearingDistance(this Position position1, double bearingRad, double distanceKm)
        {
            var distNorm = distanceKm / GeoConstants.EarthRadiusKm;
            var cosDistNorm = Math.Cos(distNorm);
            var sinDistNorm = Math.Sin(distNorm);
            var cosLat1 = Math.Cos(position1.Latitude.ToRadians());
            var sinLat1 = Math.Sin(position1.Latitude.ToRadians());

            // http://www.movable-type.co.uk/scripts/latlong.html
            var lat2 = Math.Asin(sinLat1 * cosDistNorm + cosLat1 * sinDistNorm * Math.Cos(bearingRad));
            var lng2 = position1.Longitude.ToRadians() + Math.Atan2(Math.Sin(bearingRad) * sinDistNorm * cosLat1,
                                     cosDistNorm - sinLat1 * Math.Sin(lat2));

            lng2 = (lng2.ToDegrees() + 540) % 360 - 180;

            return new Position(lat2.ToDegrees(), lng2);
        }

        public static Position TranslateToDirectionDistance(this Position position1, Position direction, double distanceKm)
        {// Could probably be simplified...
            var bearing = new Interfaces.SimpleImplementations.SimpleSegment() { Position1 = position1, Position2 = direction }.ToBearingRad();
            return TranslateToBearingDistance(position1, bearing, distanceKm);
        }

        public static Position TranslateInterpolateLinear(this Position from, Position to, float fraction)
        {
            double lat = (to.Latitude - from.Latitude) * fraction + from.Latitude;
            double lng = (to.Longitude - from.Longitude) * fraction + from.Longitude;
            return new Position(lat, lng);
        }

        public static Position TranslateInterpolateLinearFixed(this Position from, Position to, float fraction)
        {
            double lat = (to.Latitude - from.Latitude) * fraction + from.Latitude;
            double lngDelta = to.Longitude - from.Longitude;

            // Take the shortest path across the 180th meridian.
            if (Math.Abs(lngDelta) > 180)
            {
                lngDelta -= Math.Sign(lngDelta) * 360;
            }
            double lng = lngDelta * fraction + from.Longitude;
            return new Position(lat, lng);
        }

        /* From github.com/googlemaps/android-maps-utils */
        public static Position TranslateInterpolateSpherical(this Position from, Position to, float fraction)
        {
            // http://en.wikipedia.org/wiki/Slerp
            double fromLat = from.Latitude.ToRadians();
            double fromLng = from.Longitude.ToRadians();
            double toLat = to.Latitude.ToRadians();
            double toLng = to.Longitude.ToRadians();
            double cosFromLat = Math.Cos(fromLat);
            double cosToLat = Math.Cos(toLat);

            // Computes Spherical interpolation coefficients.
            double angle = GeoExtensions.AngleBetween(fromLat, fromLng, toLat, toLng);
            double sinAngle = Math.Sin(angle);
            if (sinAngle < 1E-6)
            {
                return from;
            }
            double a = Math.Sin((1 - fraction) * angle) / sinAngle;
            double b = Math.Sin(fraction * angle) / sinAngle;

            // Converts from polar to vector and interpolate.
            double x = a * cosFromLat * Math.Cos(fromLng) + b * cosToLat * Math.Cos(toLng);
            double y = a * cosFromLat * Math.Sin(fromLng) + b * cosToLat * Math.Sin(toLng);
            double z = a * Math.Sin(fromLat) + b * Math.Sin(toLat);

            // Converts interpolated vector back to polar.
            double lat = Math.Atan2(z, Math.Sqrt(x * x + y * y));
            double lng = Math.Atan2(y, x);
            return new Position(lat.ToDegrees(), lng.ToDegrees());
        }

        #endregion Position translations        

        #region Segment tools

        public static double LengthKm(this ISegment s) => s.Position2.KmTo(s.Position1);

        // To check if same as bearing...
        public static double AngleBetween(double fromLatRad, double fromLngRad, double toLatRad, double toLngRad)
        { // https://gist.github.com/broady/6314689
            // Haversine's formula
            double dLat = fromLatRad - toLatRad;
            double dLng = fromLngRad - toLngRad;
            return 2 * Math.Asin(Math.Sqrt(
                Math.Pow(Math.Sin(dLat / 2), 2) +
                    Math.Cos(fromLatRad) * Math.Cos(toLatRad) * 
                    Math.Pow(Math.Sin(dLng / 2), 2)));
        }

        /// <summary>
        /// Angle between the segment and constant longitude
        /// </summary>
        public static double ToBearingRad(this ISegment s)
        {
            return ToBearingRad(s.Position1.Latitude.ToRadians(), s.Position1.Longitude.ToRadians(), s.Position2.Latitude.ToRadians(), s.Position2.Longitude.ToRadians());
        }
        /// <summary>
        /// Angle between the segment and constant longitude
        /// 
        /// This formula is for the initial bearing (sometimes referred to as forward azimuth) which if followed in a straight line along a great-circle arc will take you from the start point to the end point
        /// <see cref="http://www.movable-type.co.uk/scripts/latlong.html"/>
        /// </summary>
        public static double ToBearingRad(double lat1, double lng1, double lat2, double lng2)
        {
            // http://www.movable-type.co.uk/scripts/latlong.html
            var y = Math.Sin(lng2 - lng1) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) -
                    Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(lng2 - lng2);

            return Math.Atan2(y, x);
        }
        public static double ToBearingDeg(this ISegment s) => ToBearingRad(s).ToDegrees();

        #endregion Segment tools

    }
}
