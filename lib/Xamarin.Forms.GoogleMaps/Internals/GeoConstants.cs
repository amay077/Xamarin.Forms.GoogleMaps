using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps.Internals
{
    internal static class GeoConstants
    {
        public const double EarthRadiusKm = 6371;
        public const double EarthCircumferenceKm = EarthRadiusKm * 2 * Math.PI;
        public const double MetersPerMile = 1609.344;
        public const double MetersPerKilometer = 1000.0;
    }
}
