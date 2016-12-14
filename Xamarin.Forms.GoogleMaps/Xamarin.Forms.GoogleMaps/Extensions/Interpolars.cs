using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps.Extensions
{
    public interface IPositionInterpolator
    {
        Position Interpolate(float fraction, Position a, Position b);
    }

    public class Linear : IPositionInterpolator
    {
        public Position Interpolate(float fraction, Position a, Position b)
        {
            double lat = (b.Latitude - a.Latitude) * fraction + a.Latitude;
            double lng = (b.Longitude - a.Longitude) * fraction + a.Longitude;
            return new Position(lat, lng);
        }
    }

    public class LinearFixed : IPositionInterpolator
    {
        public Position Interpolate(float fraction, Position a, Position b)
        {
            double lat = (b.Latitude - a.Latitude) * fraction + a.Latitude;
            double lngDelta = b.Longitude - a.Longitude;

            // Take the shortest path across the 180th meridian.
            if (Math.Abs(lngDelta) > 180)
            {
                lngDelta -= Math.Sign(lngDelta) * 360;
            }
            double lng = lngDelta * fraction + a.Longitude;
            return new Position(lat, lng);
        }
    }

    public class Spherical : IPositionInterpolator
    {

        /* From github.com/googlemaps/android-maps-utils */
        public Position Interpolate(float fraction, Position from, Position to)
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

    }
}