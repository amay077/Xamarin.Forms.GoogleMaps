﻿using System;
using System.ComponentModel;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Java.Lang;
using Xamarin.Forms.Platform.Android;
using Math = System.Math;
using Android.Util;
using Android.App;
using Xamarin.Forms.GoogleMaps.Logics.Android;
using Xamarin.Forms.GoogleMaps.Logics;
using Xamarin.Forms.GoogleMaps.Android.Extensions;
using Android.Widget;
using Android.Views;

using GCameraUpdateFactory = Android.Gms.Maps.CameraUpdateFactory;
using GCameraPosition = Android.Gms.Maps.Model.CameraPosition;

namespace Xamarin.Forms.GoogleMaps.Android
{
    public class MapRenderer : ViewRenderer,
        GoogleMap.IOnCameraChangeListener,
        GoogleMap.IOnMapClickListener,
        GoogleMap.IOnMapLongClickListener,
        GoogleMap.IOnMyLocationButtonClickListener
    {
        readonly CameraLogic _cameraLogic = new CameraLogic();
        readonly BaseLogic<GoogleMap>[] _logics;

        public MapRenderer() : base()
        {
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
                foreach (var logic in _logics)
                    logic.ScaledDensity = metrics.ScaledDensity;

                _cameraLogic.ScaledDensity = metrics.ScaledDensity;
            }

            if (e.OldElement != null)
            {
                var oldMapModel = (Map)e.OldElement;

                _cameraLogic.Unregister();

                var oldGoogleMap = await oldMapView.GetGoogleMapAsync();
                if (oldGoogleMap != null)
                {

                    oldGoogleMap.SetOnCameraChangeListener(null);
                    oldGoogleMap.SetOnMapClickListener(null);
                    oldGoogleMap.SetOnMapLongClickListener(null);
                    oldGoogleMap.SetOnMyLocationButtonClickListener(null);
                }

                oldMapView.Dispose();
            }

            if (oldMapView != null)
            {
                _oldNativeMap = await oldMapView.GetGoogleMapAsync();
                _oldMap = (Map)e.OldElement;
            }

            NativeMap = await ((MapView)Control).GetGoogleMapAsync();

            _cameraLogic.Register(Map, NativeMap);

            OnMapReady(NativeMap);
        }

        void OnMapReady(GoogleMap map)
        {
            if (map != null)
            {
                map.SetOnCameraChangeListener(this);
                map.SetOnMapClickListener(this);
                map.SetOnMapLongClickListener(this);
                map.SetOnMyLocationButtonClickListener(this);
                map.UiSettings.MapToolbarEnabled = false;
                map.UiSettings.ZoomControlsEnabled = Map.HasZoomEnabled;
                map.UiSettings.ZoomGesturesEnabled = Map.HasZoomEnabled;
                map.UiSettings.ScrollGesturesEnabled = Map.HasScrollEnabled;
                map.MyLocationEnabled = map.UiSettings.MyLocationButtonEnabled = Map.IsShowingUser;
                map.TrafficEnabled = Map.IsTrafficEnabled;
                SetMapType();
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

            if (NativeMap == null)
                return;

            if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
                NativeMap.MyLocationEnabled = NativeMap.UiSettings.MyLocationButtonEnabled = Map.IsShowingUser;
            else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
                NativeMap.UiSettings.ScrollGesturesEnabled = Map.HasScrollEnabled;
            else if (e.PropertyName == Map.HasZoomEnabledProperty.PropertyName)
            {
                NativeMap.UiSettings.ZoomControlsEnabled = Map.HasZoomEnabled;
                NativeMap.UiSettings.ZoomGesturesEnabled = Map.HasZoomEnabled;
            }
            else if (e.PropertyName == Map.IsTrafficEnabledProperty.PropertyName)
            {
                NativeMap.TrafficEnabled = Map.IsTrafficEnabled;
            }

            foreach (var logic in _logics)
                logic.OnMapPropertyChanged(e);
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
                case MapType.None:
                    map.MapType = GoogleMap.MapTypeNone;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnCameraChange(GCameraPosition pos)
        {
            UpdateVisibleRegion(pos.Target);
            var camera = pos.ToXamarinForms();
            Map.CameraPosition = camera;
            Map.SendCameraChanged(camera);
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

                _cameraLogic.Unregister();

                foreach (var logic in _logics)
                    logic.Unregister(NativeMap, Map);

                if (NativeMap != null)
                {
                    NativeMap.MyLocationEnabled = false;
                    NativeMap.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}
