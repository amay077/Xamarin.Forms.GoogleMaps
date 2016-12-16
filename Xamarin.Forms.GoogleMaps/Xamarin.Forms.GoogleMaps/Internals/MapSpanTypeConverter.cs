using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps.Internals
{
    public class MapSpanTypeConverter : TypeConverter
    {
        const string REG_CENTER_DELTAS = @"^\((?<lat>[0-9\.]+),(?<lng>[0-9\.]+)\),(?<latDelta>[0-9\.]+),(?<lngDelta>[0-9\.]+)$";
        const string REG_NORTHWEST_SOUTHEAST = @"^\((?<north>[0-9\.]+),(?<west>[0-9\.]+)\),\((?<south>[0-9\.]+),(?<east>[0-9\.]+)\)$";

        public override object ConvertFromInvariantString(string value)
        {
            if (value != null)
            {
                if (Regex.IsMatch(value, REG_CENTER_DELTAS))
                { // (latCenter,lngCenter),deltaLat,deltaLng)
                    var reg = Regex.Match(value, REG_CENTER_DELTAS);
                    double lat = double.Parse(reg.Groups["lat"].Value);
                    double lng = double.Parse(reg.Groups["lng"].Value);
                    double latDelta = double.Parse(reg.Groups["latDelta"].Value);
                    double lngDelta = double.Parse(reg.Groups["lngDelta"].Value);
                    return new MapSpan(new Position(lat, lng), latDelta, lngDelta);
                }
                else if (Regex.IsMatch(value, REG_NORTHWEST_SOUTHEAST))
                { // (latTop,lngLeft),(latBottom,LngRight)
                    var reg = Regex.Match(value, REG_NORTHWEST_SOUTHEAST);
                    double north = double.Parse(reg.Groups["north"].Value);
                    double west = double.Parse(reg.Groups["west"].Value);
                    double south = double.Parse(reg.Groups["south"].Value);
                    double east = double.Parse(reg.Groups["east"].Value);
                    return new MapSpan(new Position((north + south) / 2, (west + east) / 2), north - south, west - east);
                }
            }

            throw new InvalidOperationException(string.Format("Cannot convert \"{0}\" into {1}", value, typeof(Thickness)));
        }
    }
}
