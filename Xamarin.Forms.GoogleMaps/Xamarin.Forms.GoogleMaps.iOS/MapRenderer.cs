using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Xamarin.Forms.Platform.iOS;
using Google.Maps;
using CoreLocation;
using System.Drawing;
using APolyline = Google.Maps.Polyline;
using APolygon = Google.Maps.Polygon;

namespace Xamarin.Forms.GoogleMaps.iOS
{
    public class MapRenderer : ViewRenderer
    {
        bool _shouldUpdateRegion;

        const string MoveMessageName = "MapMoveToRegion";

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
                    MessagingCenter.Unsubscribe<Map, MapSpan>(this, MoveMessageName);
                    ((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged -= OnPinCollectionChanged;
                    ((ObservableCollection<Polyline>)mapModel.Polylines).CollectionChanged -= OnPolylineCollectionChanged;
                    ((ObservableCollection<Polygon>)mapModel.Polygons).CollectionChanged -= OnPolygonCollectionChanged;
                }

                var mkMapView = (MapView)Control;
                mkMapView.InfoTapped -= InfoWindowTapped;
                mkMapView.OverlayTapped -= OverlayTapped;
                mkMapView.CameraPositionChanged -= CameraPositionChanged;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var mapModel = (Map)e.OldElement;
                MessagingCenter.Unsubscribe<Map, MapSpan>(this, "MapMoveToRegion");
                ((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged -= OnPinCollectionChanged;
                ((ObservableCollection<Polyline>)mapModel.Pins).CollectionChanged -= OnPolylineCollectionChanged;
            }

            if (e.NewElement != null)
            {
                var mapModel = (Map)e.NewElement;

                if (Control == null)
                {
                    SetNativeControl(new MapView(RectangleF.Empty));
                    var mkMapView = (MapView)Control;
                    //var mapDelegate = new MapDelegate(mapModel);
                    //mkMapView.GetViewForAnnotation = mapDelegate.GetViewForAnnotation;
                    mkMapView.CameraPositionChanged += CameraPositionChanged;
                    mkMapView.InfoTapped += InfoWindowTapped;
                    mkMapView.OverlayTapped += OverlayTapped;
                }

                MessagingCenter.Subscribe<Map, MapSpan>(this, MoveMessageName, (s, a) => MoveToRegion(a), mapModel);
                if (mapModel.LastMoveToRegion != null)
                    MoveToRegion(mapModel.LastMoveToRegion, false);

                UpdateMapType();
                UpdateIsShowingUser();
                UpdateHasScrollEnabled();
                UpdateHasZoomEnabled();

                ((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged += OnPinCollectionChanged;
                OnPinCollectionChanged(((Map)Element).Pins, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                ((ObservableCollection<Polyline>)mapModel.Polylines).CollectionChanged += OnPolylineCollectionChanged;
                OnPolylineCollectionChanged(((Map)Element).Polylines, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

                ((ObservableCollection<Polygon>)mapModel.Polygons).CollectionChanged += OnPolygonCollectionChanged;
                OnPolygonCollectionChanged(((Map)Element).Polygons, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
            }
        }

        void InfoWindowTapped(object sender, GMSMarkerEventEventArgs e)
        {
            var map = (Map)Element;

            // clicked marker
            var marker = e.Marker;

            // lookup pin
            Pin targetPin = null;
            for (var i = 0; i < map.Pins.Count; i++)
            {
                var pin = map.Pins[i];
                if (!Object.ReferenceEquals(pin.Id, marker))
                    continue;

                targetPin = pin;
                break;
            }

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetPin?.SendTap();
        }

        void OverlayTapped(object sender, GMSOverlayEventEventArgs e)
        {
            var map = (Map)Element;

            // clicked marker
            var overlay = e.Overlay;

            if (overlay is APolyline)
            {
                // lookup pin
                Polyline targetPolyline = null;
                for (var i = 0; i < map.Polylines.Count; i++)
                {
                    var line = map.Polylines[i];
                    if (!Object.ReferenceEquals(line.Id, overlay))
                        continue;

                    targetPolyline = line;
                    break;
                }

                // only consider event handled if a handler is present. 
                // Else allow default behavior of displaying an info window.
                targetPolyline?.SendTap();
            }
            else if (overlay is APolygon) 
            {
                // lookup pin
                Polygon targetPolygon = null;
                for (var i = 0; i < map.Polygons.Count; i++) {
                    var polygon = map.Polygons[i];
                    if (!Object.ReferenceEquals (polygon.Id, overlay))
                        continue;

                    targetPolygon = polygon;
                    break;
                }

                // only consider event handled if a handler is present. 
                // Else allow default behavior of displaying an info window.
                targetPolygon?.SendTap ();
            }

        }

        void UpdateSelectedPin(Pin pin)
        {
            var mapView = (MapView)Control;

            if (pin != null)
                mapView.SelectedMarker = (Marker)pin.Id;
            else
                mapView.SelectedMarker = null;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var mapModel = (Map)Element;

            if (e.PropertyName == Map.MapTypeProperty.PropertyName)
                UpdateMapType();
            else if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
                UpdateIsShowingUser();
            else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
                UpdateHasScrollEnabled();
            else if (e.PropertyName == Map.HasZoomEnabledProperty.PropertyName)
                UpdateHasZoomEnabled();
            else if (e.PropertyName == VisualElement.IsVisibleProperty.PropertyName && ((Map)Element).LastMoveToRegion != null)
                _shouldUpdateRegion = true;
            else if (e.PropertyName == Map.SelectedPinProperty.PropertyName)
                UpdateSelectedPin(mapModel.SelectedPin);
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

        void AddPins(IList pins)
        {
            foreach (Pin pin in pins)
            {
                var marker = Marker.FromPosition(new CLLocationCoordinate2D(pin.Position.Latitude, pin.Position.Longitude));
                marker.Title = pin.Label;
                marker.Snippet = pin.Address ?? string.Empty;
                pin.Id = marker;
                marker.Map = (MapView)Control;
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

        void OnPinCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddPins(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemovePins(notifyCollectionChangedEventArgs.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemovePins(notifyCollectionChangedEventArgs.OldItems);
                    AddPins(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    var mapView = (MapView)Control;
                    mapView.Clear();
                    AddPins((IList)(Element as Map).Pins);
                    break;
                case NotifyCollectionChangedAction.Move:
                    //do nothing
                    break;
            }
        }


        void OnPolylineCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddPolylines(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemovePolylines(notifyCollectionChangedEventArgs.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemovePolylines(notifyCollectionChangedEventArgs.OldItems);
                    AddPolylines(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    var mapView = (MapView)Control;
                    mapView.Clear(); // TODO need?
                    AddPolylines((IList)(Element as Map).Polylines);
                    break;
                case NotifyCollectionChangedAction.Move:
                    //do nothing
                    break;
            }
        }

        void OnPolygonCollectionChanged (object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action) {
            case NotifyCollectionChangedAction.Add:
                AddPolygons (notifyCollectionChangedEventArgs.NewItems);
                break;
            case NotifyCollectionChangedAction.Remove:
                RemovePolygons (notifyCollectionChangedEventArgs.OldItems);
                break;
            case NotifyCollectionChangedAction.Replace:
                RemovePolygons (notifyCollectionChangedEventArgs.OldItems);
                AddPolygons (notifyCollectionChangedEventArgs.NewItems);
                break;
            case NotifyCollectionChangedAction.Reset:
                var mapView = (MapView)Control;
                mapView.Clear (); // TODO need?
                AddPolygons ((IList)(Element as Map).Polygons);
                break;
            case NotifyCollectionChangedAction.Move:
                //do nothing
                break;
            }
        }


        void RemovePins(IList pins)
        {
            foreach (object pin in pins)
                ((Marker)((Pin)pin).Id).Map = null;
        }

        void AddPolylines(IList polylines)
        {
            foreach (Polyline polyline in polylines)
            {
                var path = new MutablePath();
                foreach (var p in polyline.Positions)
                    path.AddLatLon(p.Latitude, p.Longitude);

                var nativePolyline = APolyline.FromPath(path);
                nativePolyline.StrokeWidth = polyline.StrokeWidth;
                nativePolyline.StrokeColor = polyline.StrokeColor.ToUIColor();
                nativePolyline.Tappable = polyline.IsClickable;

                polyline.Id = nativePolyline;
                nativePolyline.Map = (MapView)Control;
            }
        }

        void RemovePolylines(IList polylines)
        {
            foreach (object obj in polylines)
                ((APolyline)((Polyline)obj).Id).Map = null;
        }

        void AddPolygons (IList polygons)
        {
            foreach (Polygon polygon in polygons) {
                var path = new MutablePath ();
                foreach (var p in polygon.Positions)
                    path.AddLatLon (p.Latitude, p.Longitude);

                var nativePolygon = APolygon.FromPath (path);
                nativePolygon.StrokeWidth = polygon.StrokeWidth;
                nativePolygon.StrokeColor = polygon.StrokeColor.ToUIColor ();
                nativePolygon.Tappable = polygon.IsClickable;

                polygon.Id = nativePolygon;
                nativePolygon.Map = (MapView)Control;
            }
        }

        void RemovePolygons(IList polygons)
        {
            foreach (object obj in polygons)
                ((APolyline)((Polygon)obj).Id).Map = null;
        }

        void UpdateHasScrollEnabled()
        {
            ((MapView)Control).Settings.ScrollGestures = ((Map)Element).HasScrollEnabled;
        }

        void UpdateHasZoomEnabled()
        {
            ((MapView)Control).Settings.ZoomGestures = ((Map)Element).HasZoomEnabled;
        }

        void UpdateIsShowingUser()
        {
            ((MapView)Control).MyLocationEnabled = ((Map)Element).IsShowingUser;
            ((MapView)Control).Settings.MyLocationButton = ((Map)Element).IsShowingUser;
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
            }
        }
    }
}