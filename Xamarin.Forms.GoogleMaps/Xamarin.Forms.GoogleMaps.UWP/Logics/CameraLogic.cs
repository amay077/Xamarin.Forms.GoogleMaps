using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.GoogleMaps.Internals;
using Xamarin.Forms.GoogleMaps.Logics;

namespace Xamarin.Forms.GoogleMaps.UWP.Logics
{
    internal class CameraLogic : BaseCameraLogic<MapControl>
    {
        public async override void OnMoveToRegionRequest(MoveToRegionMessage m)
        {
            await MoveToRegion(m.Span, m.Animate ? MapAnimationKind.Bow : MapAnimationKind.None);
        }

        internal async Task MoveToRegion(MapSpan span, MapAnimationKind animation = MapAnimationKind.Bow)
        {
            var nw = new BasicGeoposition
            {
                Latitude = span.Center.Latitude + span.LatitudeDegrees / 2,
                Longitude = span.Center.Longitude - span.LongitudeDegrees / 2
            };
            var se = new BasicGeoposition
            {
                Latitude = span.Center.Latitude - span.LatitudeDegrees / 2,
                Longitude = span.Center.Longitude + span.LongitudeDegrees / 2
            };
            var boundingBox = new GeoboundingBox(nw, se);
            await _nativeMap.TrySetViewBoundsAsync(boundingBox, null, animation);
        }

        public override void OnMoveCameraRequest(CameraUpdateMessage m)
        {
            throw new NotImplementedException();
        }

    }
}
