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

        const double ToRadiansFactorD = 1.0d / 180d * (double)Math.PI;

        public static double ToRadians(this double val)
            => ToRadiansFactorD * val;

        public static bool IsNullOrEmpty(Position ll) { if (ll == null) return true; if (ll.Latitude == 0 || ll.Longitude == 0) return true; return false; }

        public static double MilesTo(this Position gp, Position gp2)
            => KmTo(gp, gp2) * GeoConstants.MetersPerKilometer / GeoConstants.MetersPerMile;
        

        public static double MetersTo(this Position gp, Position gp2)
            => KmTo(gp, gp2) * GeoConstants.MetersPerKilometer;

        public static double KmTo(this Position gp, Position gp2)
        {
            if (gp2 == null) return double.MaxValue;
            return KmTo(gp, gp2.Latitude, gp2.Longitude);
        }
        public static double KmTo(this Position gp, double lat2, double lng2)
        {
            var latR1 = gp.Latitude.ToRadians();
            var latR2 = lat2.ToRadians();

            var demiDLat = (latR2 - latR1) / 2;
            var demiDLon = (lng2.ToRadians() - gp.Longitude.ToRadians()) / 2;

            var a = Math.Sin(demiDLat) * Math.Sin(demiDLat) +
                Math.Cos(latR1) * Math.Cos(latR2) *
                Math.Sin(demiDLon) * Math.Sin(demiDLon);
            var c = Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return c * 2 * EarthRadiusKm;
        }

        public static bool IsIn(this Position pos, ICircle c)
            => pos.KmTo(c.CircleCenter) <= c.CircleRadius.Kilometers;
        public static bool IsIn(this Position pos, Circle c)
            => pos.KmTo(c.Center) <= c.Radius.Kilometers;

        public static bool Contains(this ICircle c, Position pos)
            => IsIn(pos, c);
        public static bool Contains(this Circle c, Position pos)
            => IsIn(pos, c);
    }
}
