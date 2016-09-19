using System;
using System.ComponentModel;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Java.Lang;
using Xamarin.Forms.Platform.Android;
using Math = System.Math;
using Android.Util;
using Android.App;
using Xamarin.Forms.GoogleMaps.Internals;
using Xamarin.Forms.GoogleMaps.Logics.Android;
using Xamarin.Forms.GoogleMaps.Logics;
using Xamarin.Forms.GoogleMaps.Android.Extensions;

namespace Xamarin.Forms.GoogleMaps.Android
{
    public class MapRenderer : ViewRenderer,
        GoogleMap.IOnCameraChangeListener,
        GoogleMap.IOnMapClickListener,
        GoogleMap.IOnMapLongClickListener
    {
        readonly BaseLogic<GoogleMap>[] _logics;

        public MapRenderer()
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

        static Bundle s_bundle;
        internal static Bundle Bundle { set { s_bundle = value; } }

        const string MoveMessageName = "MapMoveToRegion";

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

        protected async override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

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
            }

            if (e.OldElement != null)
            {
                var oldMapModel = (Map)e.OldElement;
                MessagingCenter.Unsubscribe<Map, MoveToRegionMessage>(this, MoveMessageName);

                var oldGoogleMap = await oldMapView.GetGoogleMapAsync();
                if (oldGoogleMap != null)
                {

                    oldGoogleMap.SetOnCameraChangeListener(null);
                    oldGoogleMap.SetOnMapClickListener(null);
                    oldGoogleMap.SetOnMapLongClickListener(null);
                }

                oldMapView.Dispose();
            }

            if (oldMapView != null)
            {
                _oldNativeMap = await oldMapView.GetGoogleMapAsync();
                _oldMap = (Map)e.OldElement;
            }

            MessagingCenter.Subscribe<Map, MoveToRegionMessage>(this, MoveMessageName, OnMoveToRegionMessage, Map);

            NativeMap = await ((MapView)Control).GetGoogleMapAsync();
            OnMapReady(NativeMap);
        }

        void OnMapReady(GoogleMap map)
        {
            if (map != null)
            {
                map.SetOnCameraChangeListener(this);
                map.SetOnMapClickListener(this);
                map.SetOnMapLongClickListener(this);
                map.UiSettings.ZoomControlsEnabled = Map.HasZoomEnabled;
                map.UiSettings.ZoomGesturesEnabled = Map.HasZoomEnabled;
                map.UiSettings.ScrollGesturesEnabled = Map.HasScrollEnabled;
                map.MyLocationEnabled = map.UiSettings.MyLocationButtonEnabled = Map.IsShowingUser;
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

        void OnMoveToRegionMessage(Map s, MoveToRegionMessage m)
        {
            MoveToRegion(m.Span, m.Animate);
        }

        void MoveToRegion(MapSpan span, bool animate)
        {
            var map = NativeMap;
            if (map == null)
                return;

            span = span.ClampLatitude(85, -85);
            var ne = new LatLng(span.Center.Latitude + span.LatitudeDegrees / 2, span.Center.Longitude + span.LongitudeDegrees / 2);
            var sw = new LatLng(span.Center.Latitude - span.LatitudeDegrees / 2, span.Center.Longitude - span.LongitudeDegrees / 2);
            var update = CameraUpdateFactory.NewLatLngBounds(new LatLngBounds(sw, ne), 0);

            try
            {
                if (animate)
                    map.AnimateCamera(update);
                else
                    map.MoveCamera(update);
            }
            catch (IllegalStateException exc)
            {
                System.Diagnostics.Debug.WriteLine("MoveToRegion exception: " + exc);
            }
        }

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

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
            MoveToRegion(((Map)Element).LastMoveToRegion, false);

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
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnCameraChange(CameraPosition pos)
        {
            UpdateVisibleRegion(pos.Target);
        }

        public void OnMapClick(LatLng point)
        {
            Map.SendMapClicked(point.ToPosition());
        }

        public void OnMapLongClick(LatLng point)
        {
            Map.SendMapLongClicked(point.ToPosition());
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

                if (this.Map != null)
                {
                    MessagingCenter.Unsubscribe<Map, MoveToRegionMessage>(this, MoveMessageName);
                }

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
