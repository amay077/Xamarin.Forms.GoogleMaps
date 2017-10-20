using System;
using CoreLocation;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.iOS.Extensions;
using Xamarin.Forms.GoogleMaps.Internals;

using GCameraUpdate = Google.Maps.CameraUpdate;
using CoreAnimation;

namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
    internal sealed class CameraLogic : BaseCameraLogic<MapView>
    {
        private readonly Action _raiseCameraPositionChanged;

        private volatile bool _isAnimate = false; // will be true while animation by _nativeMap.Animate
        private volatile bool _raiseWillMoveFromMethod = false; // will be true between called _nativeMap.Animate and raised NativeMap_WillMove
        private volatile bool _isCancelAnimate = false; // will be true if raised NativeMap_WillMove while animation by _nativeMap.Animate

        public CameraLogic(Action raiseCameraPositionChanged)
        {
            _raiseCameraPositionChanged = raiseCameraPositionChanged;
        }

        public override void Register(Map map, MapView nativeMap)
        {
            base.Register(map, nativeMap);
            _nativeMap.WillMove += NativeMap_WillMove;
            _nativeMap.CameraPositionChanged += NativeMap_CameraPositionChanged;
            _nativeMap.CameraPositionIdle += NativeMap_CameraPositionIdle;
        }

        public override void Unregister()
        {
            _nativeMap.CameraPositionIdle -= NativeMap_CameraPositionIdle;
            _nativeMap.CameraPositionChanged -= NativeMap_CameraPositionChanged;
            _nativeMap.WillMove -= NativeMap_WillMove;
            base.Unregister();
        }

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
            {
                _nativeMap.Animate(GCameraUpdate.FitBounds(mapRegion));
            }
            else
            {
                _nativeMap.MoveCamera(GCameraUpdate.FitBounds(mapRegion));

                // TODO WORKARROUND for CameraPositionChanged does not raise when call MoveCamera with CameraUpdate.FitBounds(issue #189)
                _raiseCameraPositionChanged?.Invoke();
            }
        }

        public override void OnMoveCameraRequest(CameraUpdateMessage m)
        {
            MoveCamera(m.Update);
            m.Callback.OnFinished();
        }

        internal void MoveCamera(CameraUpdate update)
        {
            _nativeMap.MoveCamera(update.ToIOS());

            // TODO WORKARROUND for CameraPositionChanged does not raise when call MoveCamera with CameraUpdate.FitBounds(issue #189)
            if (update.UpdateType == CameraUpdateType.LatLngBounds)
            {
                _raiseCameraPositionChanged?.Invoke();
            }
        }

        public override void OnAnimateCameraRequest(CameraUpdateMessage m)
        {
            _isCancelAnimate = _isAnimate;

            CATransaction.Begin();

            if (m.Duration.HasValue)
            {
                CATransaction.AnimationDuration = m.Duration.Value.TotalSeconds;
            }

            CATransaction.CompletionBlock = () => 
            {
                _isAnimate = false;

                if (_isCancelAnimate)
                {
                    m.Callback.OnCanceled();
                }
                else
                {
                    m.Callback.OnFinished();
                }
                _isCancelAnimate = false;
            };
            _nativeMap.Animate(m.Update.ToIOS());

            _isAnimate = true;
            _raiseWillMoveFromMethod = true;
            CATransaction.Commit();
        }

        void NativeMap_WillMove (object sender, GMSWillMoveEventArgs e)
        {
            _map.SendCameraMoveStarted(e.Gesture);
            
            // Skip the first event because Animate method causes first WillMove.
            if (_raiseWillMoveFromMethod)
            {
                _raiseWillMoveFromMethod = false;
                return;
            }

            // If dragging map when animation by Animate method then should call CallBack.OnCanceled.
            if (_isAnimate)
            {
                _isCancelAnimate = true;
            }
        }

        void NativeMap_CameraPositionChanged(object sender, GMSCameraEventArgs e)
        {
            _map.SendCameraMoving(e.Position.ToXamarinForms());
        }

        void NativeMap_CameraPositionIdle(object sender, GMSCameraEventArgs e)
        {
            _map.SendCameraIdled(e.Position.ToXamarinForms());
        }
    }
}