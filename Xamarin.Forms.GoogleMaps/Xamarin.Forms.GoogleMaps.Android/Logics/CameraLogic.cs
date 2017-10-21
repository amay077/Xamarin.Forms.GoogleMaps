using System;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Java.Lang;
using Xamarin.Forms.GoogleMaps.Android.Extensions;
using Xamarin.Forms.GoogleMaps.Android.Logics;
using Xamarin.Forms.GoogleMaps.Internals;
using static Android.Gms.Maps.GoogleMap;
using GCameraPosition = Android.Gms.Maps.Model.CameraPosition;

using GCameraUpdateFactory = Android.Gms.Maps.CameraUpdateFactory;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal sealed class CameraLogic : BaseCameraLogic<GoogleMap>
    {
        private readonly Action<LatLng> _updateVisibleRegion;

        public CameraLogic(Action<LatLng> updateVisibleRegion)
        {
            _updateVisibleRegion = updateVisibleRegion;
        }

        public override void Register(Map map, GoogleMap nativeMap)
        {
            base.Register(map, nativeMap);

            UnsubscribeCameraEvents(_nativeMap);

            nativeMap.CameraChange += NativeMap_CameraChange;
            nativeMap.CameraMoveStarted += NativeMap_CameraMoveStarted;
            nativeMap.CameraMove += NativeMap_CameraMove;
            nativeMap.CameraIdle += NativeMap_CameraIdle;
        }

        public override void Unregister()
        {
            UnsubscribeCameraEvents(_nativeMap);
            base.Unregister();
        }

        private void UnsubscribeCameraEvents(GoogleMap nativeMap)
        {
            if (nativeMap == null)
            {
                return;
            }
                
            
            nativeMap.CameraChange -= NativeMap_CameraChange;
            nativeMap.CameraMoveStarted -= NativeMap_CameraMoveStarted;
            nativeMap.CameraMove -= NativeMap_CameraMove;
            nativeMap.CameraIdle -= NativeMap_CameraIdle;
        }

        public override void OnMoveToRegionRequest(MoveToRegionMessage m)
        {
            if (_nativeMap == null)
                return;

            var span = m.Span;
            var animate = m.Animate;

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
            MoveCamera(m.Update);
            m.Callback.OnFinished();
        }

        internal void MoveCamera(CameraUpdate update)
        {
            _nativeMap.MoveCamera(update.ToAndroid(ScaledDensity));
        }

        public override void OnAnimateCameraRequest(CameraUpdateMessage m)
        {
            var update = m.Update.ToAndroid(ScaledDensity);
            var callback = new DelegateCancelableCallback(
                    () => m.Callback.OnFinished(),
                    () => m.Callback.OnCanceled());

            if (m.Duration.HasValue)
            {
                _nativeMap.AnimateCamera(update, (int)m.Duration.Value.TotalMilliseconds, callback);
            }
            else
            {
                _nativeMap.AnimateCamera(update, callback);
            }
        }

        void NativeMap_CameraChange(object sender, CameraChangeEventArgs e)
        {
            _updateVisibleRegion?.Invoke(e.Position.Target);
            var camera = e.Position.ToXamarinForms();
            _map.CameraPosition = camera;
            _map.SendCameraChanged(camera);
        }

        void NativeMap_CameraMoveStarted(object sender, GoogleMap.CameraMoveStartedEventArgs e)
        {
            // see https://developers.google.com/maps/documentation/android-api/events#camera_change_events
            _map.SendCameraMoveStarted(e.Reason == OnCameraMoveStartedListener.ReasonGesture);
        }

        void NativeMap_CameraMove(object sender, System.EventArgs e)
        {
            _map.SendCameraMoving(_nativeMap.CameraPosition.ToXamarinForms());
        }

        void NativeMap_CameraIdle(object sender, System.EventArgs e)
        {
            _map.SendCameraIdled(_nativeMap.CameraPosition.ToXamarinForms());
        }
    }
}