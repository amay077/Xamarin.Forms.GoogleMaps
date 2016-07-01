﻿using System;
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
using ACircle = Google.Maps.Circle;
using ATileLayer = Google.Maps.TileLayer;
using AUrlTileLayer = Google.Maps.UrlTileLayer;
using Xamarin.Forms.GoogleMaps.Internals;
using Foundation;

namespace Xamarin.Forms.GoogleMaps.iOS
{
    public class MapRenderer : ViewRenderer
    {
        bool _shouldUpdateRegion;

        const string MoveMessageName = "MapMoveToRegion";

        private volatile bool _onMarkerEvent = false;

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
                    ((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged -= OnPinCollectionChanged;
                    ((ObservableCollection<Polyline>)mapModel.Polylines).CollectionChanged -= OnPolylineCollectionChanged;
                    ((ObservableCollection<Polygon>)mapModel.Polygons).CollectionChanged -= OnPolygonCollectionChanged;
                    ((ObservableCollection<Circle>)mapModel.Circles).CollectionChanged -= OnCircleCollectionChanged;
					((ObservableCollection<TileLayer>)mapModel.TileLayers).CollectionChanged -= OnTileLayerCollectionChanged;
                }

                var mkMapView = (MapView)Control;
                mkMapView.InfoClosed -= InfoWindowClosed;
                mkMapView.InfoTapped -= InfoWindowTapped;
                mkMapView.OverlayTapped -= OverlayTapped;
                mkMapView.CameraPositionChanged -= CameraPositionChanged;
                mkMapView.TappedMarker = null;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                var mapModel = (Map)e.OldElement;
                MessagingCenter.Unsubscribe<Map, MoveToRegionMessage>(this, "MapMoveToRegion");
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
                    mkMapView.InfoClosed += InfoWindowClosed;
                    mkMapView.OverlayTapped += OverlayTapped;
                    mkMapView.TappedMarker = HandleGMSTappedMarker;
                }

                MessagingCenter.Subscribe<Map, MoveToRegionMessage>(this, MoveMessageName, (s, a) => MoveToRegion(a.Span, a.Animate), mapModel);
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

                ((ObservableCollection<Circle>)mapModel.Circles).CollectionChanged += OnCircleCollectionChanged;
                OnCircleCollectionChanged(((Map)Element).Circles, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

				((ObservableCollection<TileLayer>)mapModel.TileLayers).CollectionChanged += OnTileLayerCollectionChanged;
				OnTileLayerCollectionChanged(((Map)Element).TileLayers, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));			
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
            else if (overlay is ACircle)
            {
                // lookup pin
                Circle targetCircle = null;
                for (var i = 0; i < map.Circles.Count; i++)
                {
                    var circle = map.Circles[i];
                    if (!Object.ReferenceEquals(circle.Id, overlay))
                        continue;

                    targetCircle = circle;
                    break;
                }

                // only consider event handled if a handler is present. 
                // Else allow default behavior of displaying an info window.
                targetCircle?.SendTap();
            }
        }

        bool HandleGMSTappedMarker(MapView mapView, Marker marker)
        {
            var map = (Map)Element;

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

            try
            {
                _onMarkerEvent = true;

                if (targetPin != null && !object.ReferenceEquals(map.SelectedPin, targetPin))
                    map.SelectedPin = targetPin;
            }
            finally
            {
                _onMarkerEvent = false;
            }

            return false;
        }

        void InfoWindowClosed(object sender, GMSMarkerEventEventArgs e)
        {
            var map = (Map)Element;

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

            try
            {
                _onMarkerEvent = true;

                if (targetPin != null)
                    if (object.ReferenceEquals(map.SelectedPin, targetPin))
                        map.SelectedPin = null;
                    //else
                    //    System.Diagnostics.Debug.WriteLine($"InfoWindowClosed - not match SelectedPin + {map?.SelectedPin?.Label ?? "null"}");
            }
            finally
            {
                _onMarkerEvent = false;
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
            {
                if (!_onMarkerEvent)
                    UpdateSelectedPin(mapModel.SelectedPin);
                mapModel.SendSelectedPinChanged(mapModel.SelectedPin);
            }
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
                    (Element as Map).SelectedPin = null;
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

        void OnCircleCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddCircles(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveCircles(notifyCollectionChangedEventArgs.OldItems);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    RemoveCircles(notifyCollectionChangedEventArgs.OldItems);
                    AddCircles(notifyCollectionChangedEventArgs.NewItems);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    var mapView = (MapView)Control;
                    mapView.Clear(); // TODO need?
                    AddCircles((IList)(Element as Map).Circles);
                    break;
                case NotifyCollectionChangedAction.Move:
                    //do nothing
                    break;
            }
        }

		void OnTileLayerCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
		{
			switch (notifyCollectionChangedEventArgs.Action)
			{
				case NotifyCollectionChangedAction.Add:
					AddTileLayers(notifyCollectionChangedEventArgs.NewItems);
					break;
				case NotifyCollectionChangedAction.Remove:
					RemoveTileLayers(notifyCollectionChangedEventArgs.OldItems);
					break;
				case NotifyCollectionChangedAction.Replace:
					RemoveTileLayers(notifyCollectionChangedEventArgs.OldItems);
					AddTileLayers(notifyCollectionChangedEventArgs.NewItems);
					break;
				case NotifyCollectionChangedAction.Reset:
					var mapView = (MapView)Control;
					mapView.Clear(); // TODO need?
					AddTileLayers((IList)(Element as Map).TileLayers);
					break;
				case NotifyCollectionChangedAction.Move:
					//do nothing
					break;
			}
		}

		void RemovePins(IList pins)
        {
            var map = (Map)Element;
            foreach (object pin in pins)
            {
                ((Marker)((Pin)pin).Id).Map = null;

                if (object.ReferenceEquals(map.SelectedPin, pin))
                    map.SelectedPin = null;
            }
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
                nativePolygon.StrokeColor = polygon.StrokeColor.ToUIColor();
                nativePolygon.FillColor = polygon.FillColor.ToUIColor();
                nativePolygon.Tappable = polygon.IsClickable;

                polygon.Id = nativePolygon;
                nativePolygon.Map = (MapView)Control;
            }
        }

        void RemovePolygons(IList polygons)
        {
            foreach (object obj in polygons)
                ((APolygon)((Polygon)obj).Id).Map = null;
        }

        void AddCircles(IList circles)
        {
            foreach (Circle circle in circles)
            {
                var nativeCircle = ACircle.FromPosition(
                    new CLLocationCoordinate2D(circle.Center.Latitude, circle.Center.Longitude), 
                    circle.Radius.Meters);
                nativeCircle.StrokeWidth = circle.StrokeWidth;
                nativeCircle.StrokeColor = circle.StrokeColor.ToUIColor();
                nativeCircle.FillColor = circle.FillColor.ToUIColor();
                //nativeCircle.Tappable = circle.IsClickable;

                circle.Id = nativeCircle;
                nativeCircle.Map = (MapView)Control;
            }
        }

        void RemoveCircles(IList circles)
        {
            foreach (object obj in circles)
                ((ACircle)((Circle)obj).Id).Map = null;
        }

		void AddTileLayers(IList tileLayers)
		{
			foreach (TileLayer tileLayer in tileLayers)
			{
				ATileLayer nativeTileLayer;

				if (tileLayer is UrlTileLayer)
				{
					var _xformObject = new WeakReference(tileLayer);
					nativeTileLayer = AUrlTileLayer.FromUrlConstructor((nuint x, nuint y, nuint zoom) => {
						var uri = ((UrlTileLayer)_xformObject.Target).TileUri((int)x, (int)y, (int)zoom);
						return new NSUrl(uri.AbsoluteUri);
					});
				}
				else if (tileLayer is SyncTileLayer)
				{
					nativeTileLayer = new NSyncTileLayer((SyncTileLayer)tileLayer);
				}
				else if (tileLayer is AsyncTileLayer)
				{
					nativeTileLayer = new NAsyncTileLayer((AsyncTileLayer)tileLayer);
				}
				else
				{
					throw new System.Exception("Unknown TileLayer type");
				}
				nativeTileLayer.TileSize = tileLayer.TileSize;

                tileLayer.NativeObject = nativeTileLayer;
				nativeTileLayer.Map = (MapView)Control;
			}
		}

		void RemoveTileLayers(IList tileLayers)
		{
			foreach (object obj in tileLayers)
				((ATileLayer)((TileLayer)obj).NativeObject).Map = null;
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