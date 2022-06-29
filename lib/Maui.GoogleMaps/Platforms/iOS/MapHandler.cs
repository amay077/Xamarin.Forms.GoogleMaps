using Google.Maps;
using UIKit;
using Maui.GoogleMaps.Internals;
using GCameraPosition = Google.Maps.CameraPosition;
using Maui.GoogleMaps.iOS;
using Maui.GoogleMaps.Logics.iOS;
using Maui.GoogleMaps.Logics;
using Microsoft.Maui.Handlers;
using Maui.GoogleMaps.iOS.Extensions;

namespace Maui.GoogleMaps.Handlers
{
    public partial class MapHandler : ViewHandler<Map, MapView>
    {
        bool _shouldUpdateRegion = true;

        // ReSharper disable once MemberCanBePrivate.Global
        protected MapView NativeMap => PlatformView;
        // ReSharper disable once MemberCanBePrivate.Global
        protected Map Map => VirtualView;

        protected internal static PlatformConfig Config { protected get; set; }

        readonly UiSettingsLogic _uiSettingsLogic = new();
        CameraLogic _cameraLogic;

        private bool _ready;

        internal IList<BaseLogic<MapView>> Logics { get; set; }

        protected override MapView CreatePlatformView()
        {
            return new MapView(CoreGraphics.CGRect.Empty);
        }

        public override void PlatformArrange(Rect rect)
        {
            base.PlatformArrange(rect);

            if (_shouldUpdateRegion && !_ready)
            {
                _cameraLogic.MoveCamera(Map.InitialCameraUpdate);
                _ready = true;
                _shouldUpdateRegion = false;
            }
        }

        protected override void ConnectHandler(MapView platformView)
        {
            Logics = new List<BaseLogic<MapView>>
            {
                new PolylineLogic(),
                new PolygonLogic(),
                new CircleLogic(),
                new PinLogic(Config.ImageFactory, OnMarkerCreating, OnMarkerCreated, OnMarkerDeleting, OnMarkerDeleted),
                new TileLayerLogic(),
                new GroundOverlayLogic(Config.ImageFactory)
            };

            _cameraLogic = new CameraLogic(() =>
            {
                OnCameraPositionChanged(NativeMap.Camera);
            });

            platformView.CameraPositionChanged += CameraPositionChanged;
            platformView.CoordinateTapped += CoordinateTapped;
            platformView.CoordinateLongPressed += CoordinateLongPressed;
            platformView.DidTapMyLocationButton = DidTapMyLocation;

            _cameraLogic.Register(Map, NativeMap);
            Map.OnSnapshot += OnSnapshot;

            //_cameraLogic.MoveCamera(mapModel.InitialCameraUpdate);
            //_ready = true;

            _uiSettingsLogic.Register(Map, NativeMap);

            _uiSettingsLogic.Initialize();

            foreach (var logic in Logics)
            {
                logic.Register(null, null, NativeMap, Map, this);
                logic.RestoreItems();
                logic.OnMapPropertyChanged(Map.SelectedPinProperty.PropertyName);
            }

            base.ConnectHandler(platformView);
        }

        protected override void DisconnectHandler(MapView platformView)
        {
            if (Map != null)
            {
                Map.OnSnapshot -= OnSnapshot;
                foreach (var logic in Logics)
                {
                    logic.Unregister(NativeMap, Map);
                }
            }
            _cameraLogic.Unregister();
            _uiSettingsLogic.Unregister();

            var mkMapView = platformView;
            if (mkMapView != null)
            {
                mkMapView.CoordinateLongPressed -= CoordinateLongPressed;
                mkMapView.CoordinateTapped -= CoordinateTapped;
                mkMapView.CameraPositionChanged -= CameraPositionChanged;
                mkMapView.DidTapMyLocationButton = null;
            }

            base.DisconnectHandler(platformView);
        }
        
        public static void MapMapType(MapHandler handler, Map map)
        {
            handler.NativeMap.MapType = map.MapType switch
            {
                MapType.Street => MapViewType.Normal,
                MapType.Satellite => MapViewType.Satellite,
                MapType.Hybrid => MapViewType.Hybrid,
                MapType.Terrain => MapViewType.Terrain,
                MapType.None => MapViewType.None,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
        public static void MapPadding(MapHandler handler, Map map)
        {
           handler.NativeMap.Padding = map.Padding.ToUIEdgeInsets();
        }
        public static void MapIsTrafficEnabled(MapHandler handler, Map map)
        {
            handler.NativeMap.TrafficEnabled = map.IsTrafficEnabled;
        }
        public static void MapIsIndoorEnabled(MapHandler handler, Map map)
        {
           handler.NativeMap.IndoorEnabled = map.IsIndoorEnabled;
        }
        public static void MapMyLocationEnabled(MapHandler handler, Map map)
        {
            handler.NativeMap.MyLocationEnabled = map.MyLocationEnabled;
        }

        public static void MapMapStyle(MapHandler handler, Map map)
        {
            if (map.MapStyle == null)
            {
                handler.NativeMap.MapStyle = null;
            }
            else
            {
                var mapStyle = Google.Maps.MapStyle.FromJson(map.MapStyle.JsonStyle, null);
                handler.NativeMap.MapStyle = mapStyle;
            }
        }

        public static void MapSelectedPin(MapHandler handler, Map map)
        {
            foreach (var logic in handler.Logics)
            {
                logic.OnMapPropertyChanged(Map.SelectedPinProperty.PropertyName);
            }
        }

        void OnSnapshot(TakeSnapshotMessage snapshotMessage)
        {
            UIGraphics.BeginImageContextWithOptions(NativeMap.Frame.Size, false, 0f);
            NativeMap.Layer.RenderInContext(UIGraphics.GetCurrentContext());
            var snapshot = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            // Why using task? Because Android side is asynchronous. 
            Task.Run(() =>
            {
                snapshotMessage.OnSnapshot.Invoke(snapshot.AsPNG().AsStream());
            });
        }

        protected void CameraPositionChanged(object sender, GMSCameraEventArgs args)
        {
            OnCameraPositionChanged(args.Position);
        }

        void OnCameraPositionChanged(GCameraPosition pos)
        {
            if (Map == null)
                return;

            var mapModel = Map;
            var mkMapView = NativeMap;

            var region = mkMapView.Projection.VisibleRegion;
            var minLat = Math.Min(Math.Min(Math.Min(region.NearLeft.Latitude, region.NearRight.Latitude), region.FarLeft.Latitude), region.FarRight.Latitude);
            var minLon = Math.Min(Math.Min(Math.Min(region.NearLeft.Longitude, region.NearRight.Longitude), region.FarLeft.Longitude), region.FarRight.Longitude);
            var maxLat = Math.Max(Math.Max(Math.Max(region.NearLeft.Latitude, region.NearRight.Latitude), region.FarLeft.Latitude), region.FarRight.Latitude);
            var maxLon = Math.Max(Math.Max(Math.Max(region.NearLeft.Longitude, region.NearRight.Longitude), region.FarLeft.Longitude), region.FarRight.Longitude);

#pragma warning disable 618
            mapModel.VisibleRegion = new MapSpan(pos.Target.ToPosition(), maxLat - minLat, maxLon - minLon);
#pragma warning restore 618

            Map.Region = mkMapView.Projection.VisibleRegion.ToRegion();

            var camera = pos.ToXamarinForms();
            Map.CameraPosition = camera;
            Map.SendCameraChanged(camera);
        }

        protected void CoordinateTapped(object sender, GMSCoordEventArgs e)
        {
            Map.SendMapClicked(e.Coordinate.ToPosition());
        }

        protected void CoordinateLongPressed(object sender, GMSCoordEventArgs e)
        {
            Map.SendMapLongClicked(e.Coordinate.ToPosition());
        }

        bool DidTapMyLocation(MapView mapView)
        {
            return Map.SendMyLocationClicked();
        }

        #region Overridable Members

        /// <summary>
        /// Call when before marker create.
        /// You can override your custom renderer for customize marker.
        /// </summary>
        /// <param name="outerItem">the pin.</param>
        /// <param name="innerItem">the marker options.</param>
        protected virtual void OnMarkerCreating(Pin outerItem, Marker innerItem)
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
    }
}
