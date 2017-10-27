﻿using System;
using System.ComponentModel;
using System.IO;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Java.Lang;
using Xamarin.Forms.Platform.Android;
using Math = System.Math;
using Android.Util;
using Android.App;
using Android.Graphics;
using Xamarin.Forms.GoogleMaps.Logics.Android;
using Xamarin.Forms.GoogleMaps.Logics;
using Xamarin.Forms.GoogleMaps.Android.Extensions;
using Android.Widget;
using Android.Views;
using Xamarin.Forms.GoogleMaps.Android.Logics;
using Xamarin.Forms.GoogleMaps.Internals;

namespace Xamarin.Forms.GoogleMaps.Android
{
    public class MapRenderer : ViewRenderer,
        GoogleMap.IOnMapClickListener,
        GoogleMap.IOnMapLongClickListener,
        GoogleMap.IOnMyLocationButtonClickListener
    {
        readonly CameraLogic _cameraLogic;
        readonly UiSettingsLogic _uiSettingsLogic = new UiSettingsLogic();
        readonly BaseLogic<GoogleMap>[] _logics;

        public MapRenderer() : base()
        {
            _cameraLogic = new CameraLogic(UpdateVisibleRegion);

            AutoPackage = false;
            _logics = new BaseLogic<GoogleMap>[]
            {
                new PolylineLogic(),
                new PolygonLogic(),
                new CircleLogic(),
                new PinLogic(),
                new TileLayerLogic(),
                new GroundOverlayLogic()
            };
        }

        public MapRenderer(IntPtr javaReference, global::Android.Runtime.JniHandleOwnership transfer) : this() { }

        static Bundle s_bundle;
        internal static Bundle Bundle { set { s_bundle = value; } }

        protected GoogleMap NativeMap { get; private set; }

        protected Map Map => (Map)Element;

        private GoogleMap _oldNativeMap;
        private Map _oldMap;

        bool _ready = false;
        bool _onLayout = false;

        float _scaledDensity;

        public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            return new SizeRequest(new Size(Context.ToPixels(40), Context.ToPixels(40)));
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            // For XAML Previewer or FormsGoogleMaps.Init not called.
            if (!FormsGoogleMaps.IsInitialized)
            {
                var tv = new TextView(Context)
                {
                    Gravity = GravityFlags.Center,
                    Text = "Xamarin.Forms.GoogleMaps"
                };
                tv.SetBackgroundColor(Color.Teal.ToAndroid());
                tv.SetTextColor(Color.Black.ToAndroid());
                SetNativeControl(tv);
                return;
            }

            var oldMapView = (MapView)Control;

            var mapView = new MapView(Context);
            mapView.OnCreate(s_bundle);
            mapView.OnResume();
            SetNativeControl(mapView);

            var activity = Context as Activity;
            if (activity != null)
            {
                var metrics = new DisplayMetrics();
                activity.WindowManager.DefaultDisplay.GetMetrics(metrics);
                _scaledDensity = metrics.ScaledDensity;
                foreach (var logic in _logics)
                {
                    logic.ScaledDensity = _scaledDensity;
                }

                _cameraLogic.ScaledDensity = _scaledDensity;
            }

            if (e.OldElement != null)
            {
                var oldMapModel = (Map)e.OldElement;

                oldMapModel.OnSnapshot -= OnSnapshot;
                _cameraLogic.Unregister();
                oldMapView.Dispose();
            }

            if (oldMapView != null)
            {
                _oldNativeMap = await oldMapView.GetGoogleMapAsync();
                _oldMap = (Map)e.OldElement;
            }

            NativeMap = await ((MapView)Control).GetGoogleMapAsync();

            if (Map == null) return;
            _cameraLogic.Register(Map, NativeMap);
            Map.OnSnapshot += OnSnapshot;

            _uiSettingsLogic.Register(Map, NativeMap);

            OnMapReady(NativeMap);
        }

        private void OnSnapshot(TakeSnapshotMessage snapshotMessage)
        {
            NativeMap.Snapshot(new DelegateSnapshotReadyCallback(snapshot =>
            {
                var stream = new MemoryStream();
                snapshot.Compress(Bitmap.CompressFormat.Png, 0, stream);
                stream.Position = 0;
                snapshotMessage?.OnSnapshot?.Invoke(stream);
            }));
        }

        void OnMapReady(GoogleMap map)
        {
            if (map != null)
            {
                _cameraLogic.Register(Map, NativeMap);

                map.SetOnMapClickListener(this);
                map.SetOnMapLongClickListener(this);
                map.SetOnMyLocationButtonClickListener(this);

                UpdateIsShowingUser(_uiSettingsLogic.MyLocationButtonEnabled);
                UpdateHasScrollEnabled(_uiSettingsLogic.ScrollGesturesEnabled);
                UpdateHasZoomEnabled(_uiSettingsLogic.ZoomControlsEnabled, _uiSettingsLogic.ZoomGesturesEnabled);
                UpdateHasRotationEnabled(_uiSettingsLogic.RotateGesturesEnabled);
                UpdateIsTrafficEnabled();
                UpdateIndoorEnabled();
                UpdateMapStyle();
                _uiSettingsLogic.Initialize();

                SetMapType();
                SetPadding();
            }

            foreach (var logic in _logics)
            {
                logic.Register(_oldNativeMap, _oldMap, NativeMap, Map);
            }

            _ready = true;
            if (_ready && _onLayout)
            {
                InitializeLogic();
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            // For XAML Previewer or FormsGoogleMaps.Init not called.
            if (!FormsGoogleMaps.IsInitialized)
            {
                return;
            }

            _onLayout = true;

            if (_ready && _onLayout)
            {
                InitializeLogic();
            }
            else if (changed && NativeMap != null)
            {
                UpdateVisibleRegion(NativeMap.CameraPosition.Target);
                _cameraLogic.MoveCamera(Map.InitialCameraUpdate);
            }
        }

        void InitializeLogic()
        {
            _cameraLogic.MoveCamera(Map.InitialCameraUpdate);
            //_cameraLogic.MoveToRegion(((Map)Element).LastMoveToRegion, false);

            foreach (var logic in _logics)
            {
                if (logic.Map != null)
                {
                    logic.RestoreItems();
                    logic.OnMapPropertyChanged(new PropertyChangedEventArgs(Map.SelectedPinProperty.PropertyName));
                }
            }

            _ready = false;
            _onLayout = false;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // For XAML Previewer or FormsGoogleMaps.Init not called.
            if (!FormsGoogleMaps.IsInitialized)
            {
                return;
            }

            if (e.PropertyName == Map.MapTypeProperty.PropertyName)
            {
                SetMapType();
                return;
            }

            if (e.PropertyName == Map.PaddingProperty.PropertyName)
            {
                SetPadding();
                return;
            }

            if (NativeMap == null)
            {
                return;
            }

            if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
            {
                UpdateIsShowingUser();
            }
            else if (e.PropertyName == Map.MyLocationEnabledProperty.PropertyName)
            {
                UpdateMyLocationEnabled();
            }
            else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
            {
                UpdateHasScrollEnabled();
            }
            else if (e.PropertyName == Map.HasZoomEnabledProperty.PropertyName)
            {
                UpdateHasZoomEnabled();
            }
            else if (e.PropertyName == Map.HasRotationEnabledProperty.PropertyName)
            {
                UpdateHasRotationEnabled();
            }
            else if (e.PropertyName == Map.IsTrafficEnabledProperty.PropertyName)
            {
                UpdateIsTrafficEnabled();
            }
            else if (e.PropertyName == Map.IndoorEnabledProperty.PropertyName)
            {
                UpdateIndoorEnabled();
            }
            else if (e.PropertyName == Map.MapStyleProperty.PropertyName)
            {
                UpdateMapStyle();
            }

            foreach (var logic in _logics)
            {
                logic.OnMapPropertyChanged(e);
            }
        }

        private void UpdateIsShowingUser(bool? initialMyLocationButtonEnabled = null)
        {
            NativeMap.MyLocationEnabled = Map.IsShowingUser;
            NativeMap.UiSettings.MyLocationButtonEnabled = initialMyLocationButtonEnabled ?? Map.IsShowingUser;
        }

        private void UpdateMyLocationEnabled(bool? initialMyLocationEnabled = null)
        {
            NativeMap.MyLocationEnabled = initialMyLocationEnabled ?? Map.MyLocationEnabled;
        }

        private void UpdateHasScrollEnabled(bool? initialScrollGesturesEnabled = null)
        {
            NativeMap.UiSettings.ScrollGesturesEnabled = initialScrollGesturesEnabled ?? Map.HasScrollEnabled;
        }

        private void UpdateHasZoomEnabled(
            bool? initialZoomControlsEnabled = null, 
            bool? initialZoomGesturesEnabled = null)
        {
            NativeMap.UiSettings.ZoomControlsEnabled = initialZoomControlsEnabled ?? Map.HasZoomEnabled;
            NativeMap.UiSettings.ZoomGesturesEnabled = initialZoomGesturesEnabled ?? Map.HasZoomEnabled;
        }

        private void UpdateHasRotationEnabled(bool? initialRotateGesturesEnabled = null)
        {
            NativeMap.UiSettings.RotateGesturesEnabled = initialRotateGesturesEnabled ?? Map.HasRotationEnabled;
        }

        private void UpdateIsTrafficEnabled()
        {
            NativeMap.TrafficEnabled = Map.IsTrafficEnabled;
        }

        private void UpdateIndoorEnabled()
        {
            NativeMap.SetIndoorEnabled(Map.IsIndoorEnabled);
        }

        private void UpdateMapStyle()
        {
            NativeMap.SetMapStyle(Map.MapStyle != null ?
                new MapStyleOptions(Map.MapStyle.JsonStyle) : 
                null);
        }

        void SetMapType()
        {
            var map = NativeMap;
            if (map == null)
                return;

            switch (Map.MapType)
            {
                case MapType.Street:
                    map.MapType = GoogleMap.MapTypeNormal;
                    break;
                case MapType.Satellite:
                    map.MapType = GoogleMap.MapTypeSatellite;
                    break;
                case MapType.Hybrid:
                    map.MapType = GoogleMap.MapTypeHybrid;
                    break;
                case MapType.Terrain:
                    map.MapType = GoogleMap.MapTypeTerrain;
                    break;
                case MapType.None:
                    map.MapType = GoogleMap.MapTypeNone;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void SetPadding()
        {
            NativeMap?.SetPadding(
                (int)(Map.Padding.Left * _scaledDensity), 
                (int)(Map.Padding.Top * _scaledDensity), 
                (int)(Map.Padding.Right * _scaledDensity),
                (int)(Map.Padding.Bottom * _scaledDensity));
        }

        public void OnMapClick(LatLng point)
        {
            Map.SendMapClicked(point.ToPosition());
        }

        public void OnMapLongClick(LatLng point)
        {
            Map.SendMapLongClicked(point.ToPosition());
        }

        public bool OnMyLocationButtonClick()
        {
            return Map.SendMyLocationClicked();
        }

        void UpdateVisibleRegion(LatLng pos)
        {
            var map = NativeMap;
            if (map == null)
                return;
            var projection = map.Projection;
            var width = Control.Width;
            var height = Control.Height;
            var ul = projection.FromScreenLocation(new global::Android.Graphics.Point(0, 0));
            var ur = projection.FromScreenLocation(new global::Android.Graphics.Point(width, 0));
            var ll = projection.FromScreenLocation(new global::Android.Graphics.Point(0, height));
            var lr = projection.FromScreenLocation(new global::Android.Graphics.Point(width, height));
            var dlat = Math.Max(Math.Abs(ul.Latitude - lr.Latitude), Math.Abs(ur.Latitude - ll.Latitude));
            var dlong = Math.Max(Math.Abs(ul.Longitude - lr.Longitude), Math.Abs(ur.Longitude - ll.Longitude));
            ((Map)Element).VisibleRegion = new MapSpan(
                    new Position(
                        pos.Latitude,
                        pos.Longitude
                    ),
                dlat,
                dlong
            );
        }

        bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                _uiSettingsLogic.Unregister();

                Map.OnSnapshot -= OnSnapshot;
                _cameraLogic.Unregister();

                foreach (var logic in _logics)
                    logic.Unregister(NativeMap, Map);

                if (NativeMap != null)
                {
                    NativeMap.SetOnMapClickListener(null);
                    NativeMap.SetOnMapLongClickListener(null);
                    NativeMap.SetOnMyLocationButtonClickListener(null);

                    NativeMap.MyLocationEnabled = false;
                    NativeMap.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
