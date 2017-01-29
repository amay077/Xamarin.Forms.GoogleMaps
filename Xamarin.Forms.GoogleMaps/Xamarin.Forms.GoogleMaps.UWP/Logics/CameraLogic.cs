using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.GoogleMaps.Internals;
using Xamarin.Forms.GoogleMaps.Logics;
using Xamarin.Forms.GoogleMaps.UWP.Extensions;

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

        public async override void OnMoveCameraRequest(CameraUpdateMessage m)
        {
            switch (m.Update.UpdateType)
            {
                case CameraUpdateType.LatLng:
                    _nativeMap.Center = m.Update.Position.ToGeopoint();
                    break;
                case CameraUpdateType.LatLngZoom:
                    _nativeMap.Center = m.Update.Position.ToGeopoint();
                    await _nativeMap.TryZoomToAsync(m.Update.Zoom);
                    break;
                case CameraUpdateType.LatLngBounds:
                    await _nativeMap.TrySetViewBoundsAsync(
                        m.Update.Bounds.ToGeoboundingBox(), null, MapAnimationKind.None);
                    break;
                case CameraUpdateType.CameraPosition:
                    _nativeMap.Center = m.Update.CameraPosition.Target.ToGeopoint();
                    await _nativeMap.TryRotateAsync(m.Update.CameraPosition.Bearing);
                    await _nativeMap.TryTiltToAsync(m.Update.CameraPosition.Tilt);
                    await _nativeMap.TryZoomToAsync(m.Update.CameraPosition.Zoom);
                    break;
                default:
                    break;
            }

            m.Callback.OnFinished();
        }

    }
}
