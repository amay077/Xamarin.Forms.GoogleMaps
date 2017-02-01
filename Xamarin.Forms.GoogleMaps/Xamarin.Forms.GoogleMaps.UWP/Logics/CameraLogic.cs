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
    internal sealed class CameraLogic : BaseCameraLogic<MapControl>
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
                    _nativeMap.ZoomLevel = m.Update.Zoom;
                    break;
                case CameraUpdateType.LatLngBounds:
                    _nativeMap.Heading = 0d;
                    await _nativeMap.TrySetViewBoundsAsync(
                        m.Update.Bounds.ToGeoboundingBox(),
                        new Windows.UI.Xaml.Thickness(m.Update.Padding), 
                        MapAnimationKind.None);
                    break;
                case CameraUpdateType.CameraPosition:
                    await _nativeMap.TrySetViewAsync(
                        m.Update.CameraPosition.Target.ToGeopoint(),
                        m.Update.CameraPosition.Zoom,
                        m.Update.CameraPosition.Bearing,
                        m.Update.CameraPosition.Tilt,
                        MapAnimationKind.None);
                    break;
                default:
                    break;
            }

            m.Callback.OnFinished();
        }

        public async override void OnAnimateCameraRequest(CameraUpdateMessage m)
        {
            bool result = false;
            switch (m.Update.UpdateType)
            {
                case CameraUpdateType.LatLng:
                    result = await _nativeMap.TrySetViewAsync(m.Update.Position.ToGeopoint());
                    break;
                case CameraUpdateType.LatLngZoom:
                    result = await _nativeMap.TrySetViewAsync(m.Update.Position.ToGeopoint(), m.Update.Zoom);
                    break;
                case CameraUpdateType.LatLngBounds:
                    _nativeMap.Heading = 0d;
                    result = await _nativeMap.TrySetViewBoundsAsync(
                        m.Update.Bounds.ToGeoboundingBox(),
                        new Windows.UI.Xaml.Thickness(m.Update.Padding),
                        MapAnimationKind.Bow);
                    break;
                case CameraUpdateType.CameraPosition:
                    result = await _nativeMap.TrySetViewAsync(
                        m.Update.CameraPosition.Target.ToGeopoint(),
                        m.Update.CameraPosition.Zoom,
                        m.Update.CameraPosition.Bearing,
                        m.Update.CameraPosition.Tilt,
                        MapAnimationKind.Bow);
                    break;
                default:
                    break;
            }

            if (result)
            {
                m.Callback.OnFinished();
            }
            else
            {
                m.Callback.OnCanceled();
            }
        }
    }
}
