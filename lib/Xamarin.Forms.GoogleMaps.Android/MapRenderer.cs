using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.GoogleMaps.Android.Extensions;
using Xamarin.Forms.GoogleMaps.Android.Logics;
using Xamarin.Forms.GoogleMaps.Internals;
using Xamarin.Forms.GoogleMaps.Logics;
using Xamarin.Forms.GoogleMaps.Logics.Android;
using Xamarin.Forms.Platform.Android;

namespace Xamarin.Forms.GoogleMaps.Android
{
    public class MapRenderer : ViewRenderer<Map, global::Android.Views.View>,
        GoogleMap.IOnMapClickListener,
        GoogleMap.IOnMapLongClickListener,
        GoogleMap.IOnMyLocationButtonClickListener
    {
        readonly CameraLogic _cameraLogic;
        readonly UiSettingsLogic _uiSettingsLogic = new UiSettingsLogic();
        
        internal IList<BaseLogic<GoogleMap>> Logics { get; }

        public MapRenderer(Context context) : base(context)
        {
            _cameraLogic = new CameraLogic(UpdateVisibleRegion);

            AutoPackage = false;
            Logics = new List<BaseLogic<GoogleMap>>
            {
                new PolylineLogic(),
                new PolygonLogic(),
                new CircleLogic(),
                new PinLogic(context, Config.BitmapDescriptorFactory, 
                    OnMarkerCreating, OnMarkerCreated, OnMarkerDeleting, OnMarkerDeleted),
                new TileLayerLogic(),
                new GroundOverlayLogic(context, Config.BitmapDescriptorFactory)
            };
        }

        static Bundle s_bundle;
        internal static Bundle Bundle { set { s_bundle = value; } }

        protected internal static PlatformConfig Config { protected get; set; }

        // ReSharper disable once MemberCanBePrivate.Global
        protected virtual GoogleMap NativeMap { get; private set; }

        // ReSharper disable once MemberCanBePrivate.Global
        protected Map Map => Element;

        private bool _ready = false;
        private bool _onLayout = false;

        private float _scaledDensity;

        public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            return new SizeRequest(new Size(Context.ToPixels(40), Context.ToPixels(40)));
        }

        protected override async void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == e.NewElement)
            {
                return;
            }

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

            // Uninitialize old view
            GoogleMap oldNativeMap = null;
            Map oldMap = null;
            if (e.OldElement != null)
            {
                try
                {
                    var oldNativeView = Control as MapView;
                    // ReSharper disable once PossibleNullReferenceException
                    oldNativeMap = await oldNativeView?.GetGoogleMapAsync();
                    oldMap = e.OldElement;
                    Uninitialize(oldNativeMap, oldMap);
                    oldNativeView?.Dispose();
                } 
                catch (System.Exception ex) 
                {
                    var message = ex.Message;
                    System.Diagnostics.Debug.WriteLine($"Uninitialize old view failed. - {message}");
                }
            }

            if (e.NewElement == null)
            {
                return;
            }

            var mapView = new MapView(Context);
            mapView.OnCreate(s_bundle);
            mapView.OnResume();
            SetNativeControl(mapView);

            var activity = Context as Activity;
            if (activity != null)
            {
                _scaledDensity = activity.GetScaledDensity();
                _cameraLogic.ScaledDensity = _scaledDensity;
                foreach (var logic in Logics)
                {
                    logic.ScaledDensity = _scaledDensity;
                }
            }

            var newMap = e.NewElement;
            NativeMap = await mapView.GetGoogleMapAsync();

            foreach (var logic in Logics)
            {
                logic.Register(oldNativeMap, oldMap, NativeMap, newMap);
                logic.ScaledDensity = _scaledDensity;
            }

            OnMapReady(NativeMap, newMap);
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

        protected virtual void OnMapReady(GoogleMap nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                _cameraLogic.Register(map, nativeMap);

                _uiSettingsLogic.Register(map, nativeMap);
                Map.OnSnapshot += OnSnapshot;

                Map.OnFromScreenLocation = point => 
                {
                    var latLng = nativeMap.Projection.FromScreenLocation(new global::Android.Graphics.Point((int)point.X, (int)point.Y));
                    return latLng.ToPosition();
                };

                Map.OnToScreenLocation = position =>
                {
                    var pt = nativeMap.Projection.ToScreenLocation(position.ToLatLng());
                    return new Point(pt.X, pt.Y);
                };

                nativeMap.SetOnMapClickListener(this);
                nativeMap.SetOnMapLongClickListener(this);
                nativeMap.SetOnMyLocationButtonClickListener(this);

                UpdateIsShowingUser(_uiSettingsLogic.MyLocationButtonEnabled);
                UpdateHasScrollEnabled(_uiSettingsLogic.ScrollGesturesEnabled);
                UpdateHasZoomEnabled(_uiSettingsLogic.ZoomControlsEnabled, _uiSettingsLogic.ZoomGesturesEnabled);
                UpdateHasRotationEnabled(_uiSettingsLogic.RotateGesturesEnabled);
                UpdateIsTrafficEnabled();
                UpdateIndoorEnabled();
                UpdateMapStyle();
                UpdateMyLocationEnabled();
                _uiSettingsLogic.Initialize();

                SetMapType();
                SetPadding();
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

            foreach (var logic in Logics)
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

            foreach (var logic in Logics)
            {
                logic.OnMapPropertyChanged(e);
            }
        }

        private void UpdateIsShowingUser(bool? initialMyLocationButtonEnabled = null)
        {
            NativeMap.MyLocationEnabled = Map.MyLocationEnabled;
            NativeMap.UiSettings.MyLocationButtonEnabled = initialMyLocationButtonEnabled ?? Map.MyLocationEnabled;
        }

        private void UpdateMyLocationEnabled()
        {
            NativeMap.MyLocationEnabled = Map.MyLocationEnabled;
        }

        private void UpdateHasScrollEnabled(bool? initialScrollGesturesEnabled = null)
        {
            NativeMap.UiSettings.ScrollGesturesEnabled = initialScrollGesturesEnabled ?? Map.UiSettings.ScrollGesturesEnabled;
        }

        private void UpdateHasZoomEnabled(
            bool? initialZoomControlsEnabled = null, 
            bool? initialZoomGesturesEnabled = null)
        {
            NativeMap.UiSettings.ZoomControlsEnabled = initialZoomControlsEnabled ?? Map.UiSettings.ZoomControlsEnabled;
            NativeMap.UiSettings.ZoomGesturesEnabled = initialZoomGesturesEnabled ?? Map.UiSettings.ZoomGesturesEnabled;
        }

        private void UpdateHasRotationEnabled(bool? initialRotateGesturesEnabled = null)
        {
            NativeMap.UiSettings.RotateGesturesEnabled = initialRotateGesturesEnabled ?? Map.UiSettings.RotateGesturesEnabled;
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

            if (map == null) return;
      
            Element.Region = map.Projection.VisibleRegion.ToRegion();
        }

        #region Overridable Members

        /// <summary>
        /// Call when before marker create.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">the marker options.</param>
        protected virtual void OnMarkerCreating(Pin outerItem, MarkerOptions innerItem)
        {
        }

        /// <summary>
        /// Call when after marker create.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">thr marker.</param>
        protected virtual void OnMarkerCreated(Pin outerItem, Marker innerItem)
        {
        }

        /// <summary>
        /// Call when before marker delete.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">thr marker.</param>
        protected virtual void OnMarkerDeleting(Pin outerItem, Marker innerItem)
        {
        }

        /// <summary>
        /// Call when after marker delete.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">thr marker.</param>
        protected virtual void OnMarkerDeleted(Pin outerItem, Marker innerItem)
        {
        }

        #endregion

        private void Uninitialize(GoogleMap nativeMap, Map map) 
        {
            try
            {
                if (nativeMap == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Uninitialize failed - {nameof(nativeMap)} is null");
                    return;
                }

                if (map == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Uninitialize failed - {nameof(map)} is null");
                    return;
                }

                _uiSettingsLogic.Unregister();

                map.OnSnapshot -= OnSnapshot;
                _cameraLogic.Unregister();

                foreach (var logic in Logics)
                {
                    logic.Unregister(nativeMap, map);
                }

                nativeMap.SetOnMapClickListener(null);
                nativeMap.SetOnMapLongClickListener(null);
                nativeMap.SetOnMyLocationButtonClickListener(null);

                nativeMap.MyLocationEnabled = false;
                nativeMap.Dispose();
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                System.Diagnostics.Debug.WriteLine($"Uninitialize failed. - {message}");
            }
        }

        bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;
                Uninitialize(NativeMap, Map);

                if (NativeMap != null)
                {
                    NativeMap.Dispose();
                    NativeMap = null;
                }

                (Control as MapView)?.OnDestroy();
            }

            base.Dispose(disposing);
        }
    }
}
