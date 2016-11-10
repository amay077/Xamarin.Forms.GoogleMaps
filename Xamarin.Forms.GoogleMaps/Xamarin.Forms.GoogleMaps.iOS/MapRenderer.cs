﻿using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.iOS;
using Google.Maps;
using CoreLocation;
using System.Drawing;
using Xamarin.Forms.GoogleMaps.Internals;
using Xamarin.Forms.GoogleMaps.Logics.iOS;
using Xamarin.Forms.GoogleMaps.Logics;
using Xamarin.Forms.GoogleMaps.iOS.Extensions;

namespace Xamarin.Forms.GoogleMaps.iOS
{
    public class MapRenderer : ViewRenderer
    {
        bool _shouldUpdateRegion = true;

        const string MoveMessageName = "MapMoveToRegion";

        protected MapView NativeMap => (MapView)Control;
        protected Map Map => (Map)Element;

        readonly BaseLogic<MapView>[] _logics;

        public MapRenderer()
        {
            _logics = new BaseLogic<MapView>[]
            {
                new PolylineLogic(),
                new PolygonLogic(),
                new CircleLogic(),
                new PinLogic(),
                new TileLayerLogic(),
                new GroundOverlayLogic()
            };
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return Control.GetSizeRequest(widthConstraint, heightConstraint);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {

                if (Element != null)
                {
                    var mapModel = (Map)Element;
                    MessagingCenter.Unsubscribe<Map, MoveToRegionMessage>(this, MoveMessageName);
                }

                foreach (var logic in _logics)
                    logic.Unregister(NativeMap, Map);

                var mkMapView = (MapView)Control;
                mkMapView.CoordinateLongPressed -= CoordinateLongPressed;
                mkMapView.CoordinateTapped -= CoordinateTapped;
                mkMapView.CameraPositionChanged -= CameraPositionChanged;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);
            var oldMapView = (MapView)Control;

            if (e.OldElement != null)
            {
                var mapModel = (Map)e.OldElement;
                MessagingCenter.Unsubscribe<Map, MoveToRegionMessage>(this, "MapMoveToRegion");
            }

            if (e.NewElement != null)
            {
                var mapModel = (Map)e.NewElement;

                if (Control == null)
                {
                    SetNativeControl(new MapView(RectangleF.Empty));
                    var mkMapView = (MapView)Control;
                    mkMapView.CameraPositionChanged += CameraPositionChanged;
                    mkMapView.CoordinateTapped += CoordinateTapped;
                    mkMapView.CoordinateLongPressed += CoordinateLongPressed;
                }

                MessagingCenter.Subscribe<Map, MoveToRegionMessage>(this, MoveMessageName, (s, a) => MoveToRegion(a.Span, a.Animate), mapModel);
                if (mapModel.LastMoveToRegion != null)
                    MoveToRegion(mapModel.LastMoveToRegion, false);

                UpdateAllBindiningProperties();

                foreach (var logic in _logics)
                {
                    logic.Register(oldMapView, (Map)e.OldElement, NativeMap, Map);
                    logic.RestoreItems();
                    logic.OnMapPropertyChanged(new PropertyChangedEventArgs(Map.SelectedPinProperty.PropertyName));
                }

            }
        }

        private void UpdateAllBindiningProperties()
        {
            //Map Settings
            UpdateMapType();
            UpdateIsShowingUser();
            UpdateIsTrafficEnabled();

            //UI Settings
            UpdateIsCompassEnabled();
            UpdateIsMyLocationButtonEnabled();
            UpdateHasRotateEnabled();
            UpdateHasScrollEnabled();
            UpdatHasTiltEnabled();
            UpdateHasZoomEnabled();

            //Platform specific settings
            UpdateConsumesGesturesInView();
            UpdateAllowScrollDuringRotateOrZoom();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var mapModel = (Map)Element;

            if (e.PropertyName == Map.MapTypeProperty.PropertyName)
            {
                UpdateMapType();
            }

            if (e.PropertyName == Map.IsTrafficEnabledProperty.PropertyName)
            {
                UpdateIsTrafficEnabled();
            }

            if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
            {
                UpdateIsShowingUser();
            }

            if (e.PropertyName == UiSettings.HasScrollEnabledProperty.PropertyName)
            {
                UpdateHasScrollEnabled();
            }

            if (e.PropertyName == UiSettings.IsCompassEnabledProperty.PropertyName)
            {
                UpdateIsCompassEnabled();
            }

            if (e.PropertyName == UiSettings.IsIndoorPickerEnabledProperty.PropertyName)
            {
                UpdateIndoorLevelPickerEnabled();
            }

            if (e.PropertyName == UiSettings.IsMyLocationButtonEnabledProperty.PropertyName)
            {
                UpdateIsMyLocationButtonEnabled();
            }

            if (e.PropertyName == UiSettings.HasRotateEnabledProperty.PropertyName)
            {
                UpdateHasRotateEnabled();
            }

            if (e.PropertyName == UiSettings.HasScrollEnabledProperty.PropertyName)
            {
                UpdatHasScrollEnabled();
            }

            if (e.PropertyName == UiSettings.HasTiltEnabledProperty.PropertyName)
            {
                UpdatHasTiltEnabled();
            }

            if (e.PropertyName == UiSettings.HasZoomEnabledProperty.PropertyName)
            {
                UpdateHasZoomEnabled();
            }

            if (e.PropertyName == UiSettings.ConsumesGesturesInViewProperty.PropertyName)
            {
                UpdateConsumesGesturesInView();
            }

            if (e.PropertyName == UiSettings.AllowScrollGesturesDuringRotateOrZoomProperty.PropertyName)
            {
                UpdateAllowScrollDuringRotateOrZoom();
            }

            if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName &&
                     ((Map) Element).LastMoveToRegion != null)
            {
                _shouldUpdateRegion = true;
            }

            foreach (var logic in _logics)
            {
                logic.OnMapPropertyChanged(e);
            }
        }

        private void UpdateIndoorLevelPickerEnabled()
        {
            ((MapView)Control).Settings.IndoorPicker = Map.UiSettings.IsIndoorPickerEnabled;
        }

        private void UpdateIsMyLocationButtonEnabled()
        {
            ((MapView)Control).Settings.MyLocationButton = Map.UiSettings.IsMyLocationButtonEnabled;
        }

        private void UpdateHasRotateEnabled()
        {
            ((MapView)Control).Settings.RotateGestures = Map.UiSettings.HasRotateEnabled;
        }

        private void UpdatHasScrollEnabled()
        {
            ((MapView)Control).Settings.ScrollGestures = Map.UiSettings.HasScrollEnabled;
        }

        private void UpdatHasTiltEnabled()
        {
            ((MapView)Control).Settings.TiltGestures = Map.UiSettings.HasTiltEnabled;
        }

        private void UpdateConsumesGesturesInView()
        {
            ((MapView)Control).Settings.ConsumesGesturesInView =
                Map.UiSettings.ConsumesGesturesInView;
        }

        private void UpdateAllowScrollDuringRotateOrZoom()
        {
            ((MapView) Control).Settings.AllowScrollGesturesDuringRotateOrZoom =
                Map.UiSettings.AllowScrollDuringRotateOrZoom;
        }

        private void UpdateIsCompassEnabled()
        {
            ((MapView)Control).Settings.CompassButton = Map.UiSettings.IsCompassEnabled;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            if (_shouldUpdateRegion)
            {
                MoveToRegion(((Map)Element).LastMoveToRegion, false);
                _shouldUpdateRegion = false;
            }
        }

        void CameraPositionChanged(object sender, GMSCameraEventArgs mkMapViewChangeEventArgs)
        {
            if (Element == null)
                return;

            var mapModel = (Map)Element;
            var mkMapView = (MapView)Control;

            var region = mkMapView.Projection.VisibleRegion;
            var minLat = Math.Min(Math.Min(Math.Min(region.NearLeft.Latitude, region.NearRight.Latitude), region.FarLeft.Latitude), region.FarRight.Latitude);
            var minLon = Math.Min(Math.Min(Math.Min(region.NearLeft.Longitude, region.NearRight.Longitude), region.FarLeft.Longitude), region.FarRight.Longitude);
            var maxLat = Math.Max(Math.Max(Math.Max(region.NearLeft.Latitude, region.NearRight.Latitude), region.FarLeft.Latitude), region.FarRight.Latitude);
            var maxLon = Math.Max(Math.Max(Math.Max(region.NearLeft.Longitude, region.NearRight.Longitude), region.FarLeft.Longitude), region.FarRight.Longitude);
            mapModel.VisibleRegion = new MapSpan(new Position((minLat + maxLat) / 2d, (minLon + maxLon) / 2d), maxLat - minLat, maxLon - minLon);
        }

        void CoordinateTapped(object sender, GMSCoordEventArgs e)
        {
            Map.SendMapClicked(e.Coordinate.ToPosition());
        }

        void CoordinateLongPressed(object sender, GMSCoordEventArgs e)
        {
            Map.SendMapLongClicked(e.Coordinate.ToPosition());
        }

        void MoveToRegion(MapSpan mapSpan, bool animated = true)
        {
            Position center = mapSpan.Center;
            var halfLat = mapSpan.LatitudeDegrees / 2d;
            var halfLong = mapSpan.LongitudeDegrees / 2d;
            var mapRegion = new CoordinateBounds(new CLLocationCoordinate2D(center.Latitude - halfLat, center.Longitude - halfLong),
                                                new CLLocationCoordinate2D(center.Latitude + halfLat, center.Longitude + halfLong));

            if (animated)
                ((MapView)Control).Animate(CameraUpdate.FitBounds(mapRegion));
            else
                ((MapView)Control).MoveCamera(CameraUpdate.FitBounds(mapRegion));
        }

        void UpdateHasScrollEnabled()
        {
            ((MapView)Control).Settings.ScrollGestures = Map.UiSettings.HasScrollEnabled;
        }

        void UpdateHasZoomEnabled()
        {
            ((MapView)Control).Settings.ZoomGestures = Map.UiSettings.HasScrollEnabled;
        }

        void UpdateIsShowingUser()
        {
            ((MapView)Control).MyLocationEnabled = ((Map)Element).IsShowingUser;
            ((MapView)Control).Settings.MyLocationButton = ((Map)Element).IsShowingUser;
        }

        void UpdateIsTrafficEnabled()
        {
            ((MapView)Control).TrafficEnabled = ((Map)Element).IsTrafficEnabled;
        }

        void UpdateMapType()
        {
            switch (((Map)Element).MapType)
            {
                case MapType.Street:
                    ((MapView)Control).MapType = MapViewType.Normal;
                    break;
                case MapType.Satellite:
                    ((MapView)Control).MapType = MapViewType.Satellite;
                    break;
                case MapType.Hybrid:
                    ((MapView)Control).MapType = MapViewType.Hybrid;
                    break;
                case MapType.None:
                    ((MapView)Control).MapType = MapViewType.None;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}