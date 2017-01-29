using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Java.Lang;
using Xamarin.Forms.GoogleMaps.Android.Extensions;
using Xamarin.Forms.GoogleMaps.Internals;

using GCameraUpdateFactory = Android.Gms.Maps.CameraUpdateFactory;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal sealed class CameraLogic : BaseCameraLogic<GoogleMap>
    {
        public override void OnMoveToRegionRequest(MoveToRegionMessage m)
        {
            MoveToRegion(m.Span, m.Animate);
        }

        internal void MoveToRegion(MapSpan span, bool animate)
        {
            if (_nativeMap == null)
                return;

            span = span.ClampLatitude(85, -85);
            var ne = new LatLng(span.Center.Latitude + span.LatitudeDegrees / 2, span.Center.Longitude + span.LongitudeDegrees / 2);
            var sw = new LatLng(span.Center.Latitude - span.LatitudeDegrees / 2, span.Center.Longitude - span.LongitudeDegrees / 2);
            var update = GCameraUpdateFactory.NewLatLngBounds(new LatLngBounds(sw, ne), 0);

            try
            {
                if (animate)
                    _nativeMap.AnimateCamera(update);
                else
                    _nativeMap.MoveCamera(update);
            }
            catch (IllegalStateException exc)
            {
                System.Diagnostics.Debug.WriteLine("MoveToRegion exception: " + exc);
            }
        }

        public override void OnMoveCameraRequest(CameraUpdateMessage m)
        {
            _nativeMap.MoveCamera(m.Update.ToAndroid(ScaledDensity));
            m.Callback.OnFinished();
        }
    }
}