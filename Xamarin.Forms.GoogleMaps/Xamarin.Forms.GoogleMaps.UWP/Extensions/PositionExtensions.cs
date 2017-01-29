using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Xamarin.Forms.GoogleMaps.UWP.Extensions
{
    internal static class PositionExtensions
    {
        public static BasicGeoposition ToBasicGeoposition(this Position self)
        {
            return new BasicGeoposition()
            {
                Latitude = self.Latitude,
                Longitude = self.Longitude
            };
        }

        public static Geopoint ToGeopoint(this Position self)
        {
            return new Geopoint(self.ToBasicGeoposition());
        }
    }
}
