using System;
using System.ComponentModel;
using Xamarin.Forms.Platform.iOS;
using Google.Maps;
using System.Drawing;
using Xamarin.Forms.GoogleMaps.Logics.iOS;
using Xamarin.Forms.GoogleMaps.Logics;
using Xamarin.Forms.GoogleMaps.iOS.Extensions;
using UIKit;
using Xamarin.Forms.GoogleMaps.Internals;
using GCameraUpdate = Google.Maps.CameraUpdate;
using GCameraPosition = Google.Maps.CameraPosition;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps.iOS
{
    public class MapRenderer : ViewRenderer
    {
        bool _shouldUpdateRegion = true;

        protected MapView NativeMap => (MapView)Control;
        protected Map Map => (Map)Element;

        readonly CameraLogic _cameraLogic;

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

            _cameraLogic = new CameraLogic(() =>
            {
                OnCameraPositionChanged(NativeMap.Camera);
            });
        }

        public override SizeRequest GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            return Control.GetSizeRequest(widthConstraint, heightConstraint);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Map.OnSnapshot -= OnSnapshot;
                _cameraLogic.Unregister();

                foreach (var logic in _logics)
                    logic.Unregister(NativeMap, Map);

                var mkMapView = (MapView)Control;
                mkMapView.CoordinateLongPressed -= CoordinateLongPressed;
                mkMapView.CoordinateTapped -= CoordinateTapped;
                mkMapView.CameraPositionChanged -= CameraPositionChanged;
                mkMapView.DidTapMyLocationButton = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            // For XAML Previewer or FormsGoogleMaps.Init not called.
            if (!FormsGoogleMaps.IsInitialized)
            {
                var label = new UILabel()
                {
                    Text = "Xamarin.Forms.GoogleMaps",
                    BackgroundColor = Color.Teal.ToUIColor(),
                    TextColor = Color.Black.ToUIColor(),
                    TextAlignment = UITextAlignment.Center
                };
                SetNativeControl(label);
                return;
            }

            var oldMapView = (MapView)Control;
            if (e.OldElement != null)
            {
                Map.OnSnapshot -= OnSnapshot;
                _cameraLogic.Unregister();
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
                    mkMapView.DidTapMyLocationButton = DidTapMyLocation;
                }

                _cameraLogic.Register(Map, NativeMap);
                Map.OnSnapshot += OnSnapshot;

                _cameraLogic.MoveCamera(mapModel.InitialCameraUpdate);

                UpdateMapType();
                UpdateIsShowingUser();
                UpdateHasScrollEnabled();
                UpdateHasZoomEnabled();
                UpdateHasRotationEnabled();
                UpdateIsTrafficEnabled();
                UpdatePadding();

                foreach (var logic in _logics)
                {
                    logic.Register(oldMapView, (Map)e.OldElement, NativeMap, Map);
                    logic.RestoreItems();
                    logic.OnMapPropertyChanged(new PropertyChangedEventArgs(Map.SelectedPinProperty.PropertyName));
                }

            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            // For XAML Previewer or FormsGoogleMaps.Init not called.
            if (!FormsGoogleMaps.IsInitialized)
            {
                return;
            }

            var mapModel = (Map)Element;

            if (e.PropertyName == Map.MapTypeProperty.PropertyName)
            {
                UpdateMapType();
            }
            else if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
            {
                UpdateIsShowingUser();
            }
            else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
            {
                UpdateHasScrollEnabled();
            }
            else if (e.PropertyName == Map.HasRotationEnabledProperty.PropertyName)
            {
                UpdateHasRotationEnabled();
            }
            else if (e.PropertyName == Map.HasZoomEnabledProperty.PropertyName)
            {
                UpdateHasZoomEnabled();
            }
            else if (e.PropertyName == Map.IsTrafficEnabledProperty.PropertyName)
            {
                UpdateIsTrafficEnabled();
            }
            else if (e.PropertyName == VisualElement.HeightProperty.PropertyName &&
                     ((Map) Element).InitialCameraUpdate != null)
            {
                _shouldUpdateRegion = true;
            }
            else if (e.PropertyName == Map.IndoorEnabledProperty.PropertyName)
            {
                UpdateHasIndoorEnabled();
            }

            else if (e.PropertyName == Map.PaddingProperty.PropertyName)
            {
                UpdatePadding();
            }

            foreach (var logic in _logics)
            {
                logic.OnMapPropertyChanged(e);
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            // For XAML Previewer or FormsGoogleMaps.Init not called.
            if (!FormsGoogleMaps.IsInitialized)
            {
                return;
            }

            if (_shouldUpdateRegion)
            {
                _cameraLogic.MoveCamera(((Map)Element).InitialCameraUpdate);
                _shouldUpdateRegion = false;
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

        void CameraPositionChanged(object sender, GMSCameraEventArgs args)
        {
            OnCameraPositionChanged(args.Position);
        }

        void OnCameraPositionChanged(GCameraPosition pos)
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
            mapModel.VisibleRegion = new MapSpan(pos.Target.ToPosition(), maxLat - minLat, maxLon - minLon);

            var camera = pos.ToXamarinForms();
            Map.CameraPosition = camera;
            Map.SendCameraChanged(camera);
        }

        void CoordinateTapped(object sender, GMSCoordEventArgs e)
        {
            Map.SendMapClicked(e.Coordinate.ToPosition());
        }

        void CoordinateLongPressed(object sender, GMSCoordEventArgs e)
        {
            Map.SendMapLongClicked(e.Coordinate.ToPosition());
        }

        bool DidTapMyLocation(MapView mapView)
        {
            return Map.SendMyLocationClicked();
        }

        void UpdateHasScrollEnabled()
        {
            ((MapView)Control).Settings.ScrollGestures = ((Map)Element).HasScrollEnabled;
        }

        void UpdateHasZoomEnabled()
        {
            ((MapView)Control).Settings.ZoomGestures = ((Map)Element).HasZoomEnabled;
        }

        void UpdateHasRotationEnabled()
        {
            ((MapView)Control).Settings.RotateGestures = ((Map)Element).HasRotationEnabled;
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

        void UpdateHasIndoorEnabled()
        {
            ((MapView) Control).IndoorEnabled = ((Map) Element).IsIndoorEnabled;
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

        void UpdatePadding()
        {
            ((MapView)Control).Padding = ((Map)Element).Padding.ToUIEdgeInsets();
        }
    }
}
