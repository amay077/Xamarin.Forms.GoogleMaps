using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace Xamarin.Forms.GoogleMaps.Extensions.UWP
{
    internal static class LngLatExtensions
    {
        public static Position ToPosition(this BasicGeoposition self)
        {
            return new Position(self.Latitude, self.Longitude);
        }
    }
}
