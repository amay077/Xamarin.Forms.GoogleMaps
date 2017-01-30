using CoreLocation;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.iOS.Extensions;
using Xamarin.Forms.GoogleMaps.Internals;

using GCameraUpdate = Google.Maps.CameraUpdate;

namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
    internal sealed class CameraLogic : BaseCameraLogic<MapView>
    {
        public override void OnMoveToRegionRequest(MoveToRegionMessage m)
        {
            MoveToRegion(m.Span, m.Animate);
        }

        internal void MoveToRegion(MapSpan mapSpan, bool animated = true)
        {
            Position center = mapSpan.Center;
            var halfLat = mapSpan.LatitudeDegrees / 2d;
            var halfLong = mapSpan.LongitudeDegrees / 2d;
            var mapRegion = new CoordinateBounds(new CLLocationCoordinate2D(center.Latitude - halfLat, center.Longitude - halfLong),
                new CLLocationCoordinate2D(center.Latitude + halfLat, center.Longitude + halfLong));

            if (animated)
                _nativeMap.Animate(GCameraUpdate.FitBounds(mapRegion));
            else
                _nativeMap.MoveCamera(GCameraUpdate.FitBounds(mapRegion));
        }

        public override void OnMoveCameraRequest(CameraUpdateMessage m)
        {
            _nativeMap.MoveCamera(m.Update.ToIOS());
            m.Callback.OnFinished();
        }
    }
}