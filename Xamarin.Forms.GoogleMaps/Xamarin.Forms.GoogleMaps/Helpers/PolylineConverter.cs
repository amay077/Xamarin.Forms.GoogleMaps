using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps.Helpers
{
    public static class PolylineConverter
    {
        public static List<Position> DecodePolyline(string encodedPolyline)
        {
            if (string.IsNullOrWhiteSpace(encodedPolyline))
                return null;

            int index = 0;
            var polylineChars = encodedPolyline.ToCharArray();
            var poly = new List<Position>();
            int currentLat = 0;
            int currentLng = 0;
            int next5Bits;

            while (index < polylineChars.Length)
            {
                int sum = 0;
                int shifter = 0;

                do
                {
                    next5Bits = polylineChars[index++] - 63;
                    sum |= (next5Bits & 31) << shifter;
                    shifter += 5;
                }
                while (next5Bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                sum = 0;
                shifter = 0;

                do
                {
                    next5Bits = polylineChars[index++] - 63;
                    sum |= (next5Bits & 31) << shifter;
                    shifter += 5;
                }
                while (next5Bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5Bits >= 32)
                {
                    break;
                }

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                var mLatLng = new Position(Convert.ToDouble(currentLat) / 100000.0, Convert.ToDouble(currentLng) / 100000.0);
                poly.Add(mLatLng);
            }

            return poly;
        }
    }
}
