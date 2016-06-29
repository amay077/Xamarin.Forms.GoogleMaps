using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Java.Lang;
using Xamarin.Forms.Platform.Android;
using Math = System.Math;
using Android.Runtime;
using System.Collections;
using APolyline = Android.Gms.Maps.Model.Polyline;
using APolygon = Android.Gms.Maps.Model.Polygon;
using ACircle = Android.Gms.Maps.Model.Circle;
using ATileOverlay = Android.Gms.Maps.Model.TileOverlay;
using Android.Util;
using Android.App;
using Xamarin.Forms.GoogleMaps.Internals;

namespace Xamarin.Forms.GoogleMaps.Android
{
    public class MapRenderer : ViewRenderer,
        GoogleMap.IOnCameraChangeListener
    {
        public MapRenderer()
        {
            AutoPackage = false;
        }

        static Bundle s_bundle;
        internal static Bundle Bundle { set { s_bundle = value; } }

        List<Marker> _markers;
        List<APolyline> _polylines;
        List<APolygon> _polygons;
        List<ACircle> _circles;
		List<ATileOverlay> _tileLayers;
        float _scaledDensity = 1;

        const string MoveMessageName = "MapMoveToRegion";

#pragma warning disable 618
        protected GoogleMap NativeMap => ((MapView)Control).Map;
#pragma warning restore 618

        protected Map Map => (Map)Element;

        private volatile bool _onMarkerEvent = false;

        public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
        {
            return new SizeRequest(new Size(Context.ToPixels(40), Context.ToPixels(40)));
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
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
                var realSize = new global::Android.Graphics.Point();
                activity.WindowManager.DefaultDisplay.GetRealSize(realSize);
                var size = new global::Android.Graphics.Point();
                activity.WindowManager.DefaultDisplay.GetSize(size);
                _scaledDensity = metrics.ScaledDensity;
            }

            if (e.OldElement != null)
            {
                var oldMapModel = (Map)e.OldElement;
                ((ObservableCollection<Pin>)oldMapModel.Pins).CollectionChanged -= OnPinCollectionChanged;
                ((ObservableCollection<Polyline>)oldMapModel.Polylines).CollectionChanged -= OnPolylineCollectionChanged;
                ((ObservableCollection<Polygon>)oldMapModel.Polygons).CollectionChanged -= OnPolygonCollectionChanged;
                ((ObservableCollection<Circle>)oldMapModel.Circles).CollectionChanged -= OnCircleCollectionChanged;
				((ObservableCollection<ITileLayer>)oldMapModel.TileLayers).CollectionChanged -= OnTileLayerCollectionChanged;

                MessagingCenter.Unsubscribe<Map, MoveToRegionMessage>(this, MoveMessageName);

#pragma warning disable 618
                if (oldMapView.Map != null)
                {
#pragma warning restore 618

#pragma warning disable 618
                    oldMapView.Map.SetOnCameraChangeListener(null);
#pragma warning restore 618
                    NativeMap.InfoWindowClick -= MapOnInfoWindowClick;
                    NativeMap.PolylineClick -= MapOnPolylineClick;
                    NativeMap.PolygonClick -= MapOnPolygonClick;
                    //NativeMap.CircleClick -= MapOnCircleClick; // Circle click is not supported.
                    NativeMap.MarkerClick -= MapOnMakerClick;
                    NativeMap.InfoWindowClose -= MapOnInfoWindowClose;
                }

                oldMapView.Dispose();
            }

            var map = NativeMap;
            if (map != null)
            {
                map.SetOnCameraChangeListener(this);
                NativeMap.InfoWindowClick += MapOnInfoWindowClick;
                NativeMap.PolylineClick += MapOnPolylineClick;
                NativeMap.PolygonClick += MapOnPolygonClick;
                //NativeMap.CircleClick += MapOnCircleClick; // Circle click is not supported.
                NativeMap.MarkerClick += MapOnMakerClick;
                NativeMap.InfoWindowClose += MapOnInfoWindowClose;

                map.UiSettings.ZoomControlsEnabled = Map.HasZoomEnabled;
                map.UiSettings.ZoomGesturesEnabled = Map.HasZoomEnabled;
                map.UiSettings.ScrollGesturesEnabled = Map.HasScrollEnabled;
                map.MyLocationEnabled = map.UiSettings.MyLocationButtonEnabled = Map.IsShowingUser;
                SetMapType();
            }

            MessagingCenter.Subscribe<Map, MoveToRegionMessage>(this, MoveMessageName, OnMoveToRegionMessage, Map);

            var inccPin = Map.Pins as INotifyCollectionChanged;
            if (inccPin != null)
                inccPin.CollectionChanged += OnPinCollectionChanged;


            var inccPolyline = Map.Polylines as INotifyCollectionChanged;
            if (inccPolyline != null)
                inccPolyline.CollectionChanged += OnPolylineCollectionChanged;

            var inccPolygon = Map.Polygons as INotifyCollectionChanged;
            if (inccPolygon != null)
                inccPolygon.CollectionChanged += OnPolygonCollectionChanged;

            var inccCircle = Map.Circles as INotifyCollectionChanged;
            if (inccCircle != null)
                inccCircle.CollectionChanged += OnCircleCollectionChanged;

			var inccTileLayer = Map.TileLayers as INotifyCollectionChanged;
			if (inccTileLayer != null)
				inccTileLayer.CollectionChanged += OnTileLayerCollectionChanged;
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
                    _markers?.ForEach(m => m.Remove());
                    _markers = null;
                    ((Map)Element).SelectedPin = null;
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
                    _polylines?.ForEach(line => line.Remove());
                    _polylines = null;
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
                _polygons?.ForEach (polygon => polygon.Remove());
                _polygons = null;
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
                    _circles?.ForEach(circle => circle.Remove());
                    _circles = null;
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
					_tileLayers?.ForEach(tileLayer => tileLayer.Remove());
					_tileLayers = null;
					AddTileLayers((IList)(Element as Map).TileLayers);
					break;
				case NotifyCollectionChangedAction.Move:
					//do nothing
					break;
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

        bool _init = true;

        protected override void OnLayout(bool changed, int l, int t, int r, int b)
        {
            base.OnLayout(changed, l, t, r, b);

            if (_init)
            {
                MoveToRegion(((Map)Element).LastMoveToRegion, false);
                OnPinCollectionChanged(((Map)Element).Pins, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                OnPolylineCollectionChanged (((Map)Element).Polylines, new NotifyCollectionChangedEventArgs (NotifyCollectionChangedAction.Reset));
                OnPolygonCollectionChanged(((Map)Element).Polygons, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                OnCircleCollectionChanged(((Map)Element).Circles, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
				OnTileLayerCollectionChanged(((Map)Element).TileLayers, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                _init = false;
            }
            else if (changed)
            {
                UpdateVisibleRegion(NativeMap.CameraPosition.Target);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Map.MapTypeProperty.PropertyName)
            {
                SetMapType();
                return;
            }

            var gmap = NativeMap;
            if (gmap == null)
                return;

            if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
                gmap.MyLocationEnabled = gmap.UiSettings.MyLocationButtonEnabled = Map.IsShowingUser;
            else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
                gmap.UiSettings.ScrollGesturesEnabled = Map.HasScrollEnabled;
            else if (e.PropertyName == Map.HasZoomEnabledProperty.PropertyName)
            {
                gmap.UiSettings.ZoomControlsEnabled = Map.HasZoomEnabled;
                gmap.UiSettings.ZoomGesturesEnabled = Map.HasZoomEnabled;
            }
            else if (e.PropertyName == Map.SelectedPinProperty.PropertyName)
            {
                if (!_onMarkerEvent)
                    UpdateSelectedPin(Map.SelectedPin);
                Map.SendSelectedPinChanged(Map.SelectedPin);
            }
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

        void AddPins(IList pins)
        {
            var map = NativeMap;
            if (map == null)
                return;

            if (_markers == null)
                _markers = new List<Marker>();

            _markers.AddRange(pins.Cast<Pin>().Select(p =>
            {
                var pin = (Pin)p;
                var opts = new MarkerOptions();
                opts.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
                opts.SetTitle(pin.Label);
                opts.SetSnippet(pin.Address);
                var marker = map.AddMarker(opts);

                // associate pin with marker for later lookup in event handlers
                pin.Id = marker;
                return marker;
            }));

            UpdateSelectedPin(Map.SelectedPin);
        }

        void UpdateSelectedPin(Pin pin)
        {
            if (pin == null)
            {
                if (_markers != null)
                    foreach (var marker in _markers)
                        marker.HideInfoWindow();
            }
            else
            {
                // lookup pin
                Marker targetMarker = null;
                foreach (var marker in _markers)
                {
                    if (((Marker)pin.Id).Id != marker.Id)
                        continue;

                    targetMarker = marker;
                    break;
                }

                if (targetMarker != null)
                    targetMarker.ShowInfoWindow();
            }
        }

        void RemovePins(IList pins)
        {
            var gmap = (Map)Element;
            var map = NativeMap;
            if (map == null)
                return;
            if (_markers == null)
                return;

            foreach (Pin p in pins)
            {
                var marker = _markers.FirstOrDefault(m => ((Marker)p.Id).Id == m.Id);
                if (marker == null)
                    continue;
                marker.Remove();
                _markers.Remove(marker);

                if (object.ReferenceEquals(gmap.SelectedPin,  p))
                    gmap.SelectedPin = null;

            }
        }

        void MapOnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs eventArgs)
        {
            // clicked marker
            var marker = eventArgs.Marker;

            // lookup pin
            Pin targetPin = null;
            for (var i = 0; i < Map.Pins.Count; i++)
            {
                var pin = Map.Pins[i];
                if (((Marker)pin.Id).Id != marker.Id)
                    continue;

                targetPin = pin;
                break;
            }

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetPin?.SendTap();
        }

        void MapOnMakerClick(object sender, GoogleMap.MarkerClickEventArgs eventArgs)
        {
            var map = (Map)Element;

            // clicked marker
            var marker = eventArgs.Marker;

            // lookup pin
            Pin targetPin = null;
            for (var i = 0; i < Map.Pins.Count; i++)
            {
                var pin = Map.Pins[i];
                if (((Marker)pin.Id).Id != marker.Id)
                    continue;

                targetPin = pin;
                break;
            }

            try
            {
                _onMarkerEvent = true;
                if (targetPin != null && !object.ReferenceEquals(targetPin, map.SelectedPin))
                    map.SelectedPin = targetPin;
            }
            finally
            {
                _onMarkerEvent = false;
            }

            eventArgs.Handled = false;
        }

        void MapOnInfoWindowClose(object sender, GoogleMap.InfoWindowCloseEventArgs eventArgs)
        {
            System.Diagnostics.Debug.WriteLine("MapOnInfoWindowClose");
            var map = (Map)Element;

            // clicked marker
            var marker = eventArgs.Marker;

            // lookup pin
            Pin targetPin = null;
            for (var i = 0; i < Map.Pins.Count; i++)
            {
                var pin = Map.Pins[i];
                if (((Marker)pin.Id).Id != marker.Id)
                    continue;

                targetPin = pin;
                break;
            }

            try
            {
                _onMarkerEvent = true;
                if (targetPin != null && object.ReferenceEquals(targetPin, map.SelectedPin))
                    map.SelectedPin = null;
            }
            finally
            {
                _onMarkerEvent = false;
            }
        }

        void MapOnPolylineClick(object sender, GoogleMap.PolylineClickEventArgs eventArgs)
        {
            // clicked polyline
            var clickedPolyline = eventArgs.Polyline;

            // lookup pin
            Polyline targetPolyline = null;
            for (var i = 0; i < Map.Polylines.Count; i++)
            {
                var line = Map.Polylines[i];
                if (((APolyline)line.Id).Id != clickedPolyline.Id)
                    continue;

                targetPolyline = line;
                break;
            }

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetPolyline?.SendTap();
        }

        void MapOnPolygonClick (object sender, GoogleMap.PolygonClickEventArgs eventArgs)
        {
            // clicked polygon
            var clickedPolygon = eventArgs.Polygon;

            // lookup pin
            Polygon targetPolygon = null;
            for (var i = 0; i < Map.Polygons.Count; i++) {
                var polygon = Map.Polygons [i];
                if (((APolygon)polygon.Id).Id != clickedPolygon.Id)
                    continue;

                targetPolygon = polygon;
                break;
            }

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetPolygon?.SendTap();
        }

        void AddPolylines(IList polylines)
        {
            var map = NativeMap;
            if (map == null)
                return;

            if (_polylines == null)
                _polylines = new List<APolyline>();

            _polylines.AddRange(polylines.Cast<Polyline>().Select(line =>
            {
                var polyline = (Polyline)line;
                var opts = new PolylineOptions();

                foreach (var p in polyline.Positions)
                    opts.Add(new LatLng(p.Latitude, p.Longitude));

                opts.InvokeWidth(polyline.StrokeWidth * _scaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps) 
                opts.InvokeColor(polyline.StrokeColor.ToAndroid());
                opts.Clickable(polyline.IsClickable);

                var nativePolyline = map.AddPolyline(opts);

                // associate pin with marker for later lookup in event handlers
                polyline.Id = nativePolyline;
                return nativePolyline;
            }));
        }

        void RemovePolylines(IList polylines)
        {
            var map = NativeMap;
            if (map == null)
                return;
            if (_polylines == null)
                return;

            foreach (Polyline polyline in polylines)
            {
                var apolyline = _polylines.FirstOrDefault(m => ((APolyline)polyline.Id).Id == m.Id);
                if (apolyline == null)
                    continue;
                apolyline.Remove();
                _polylines.Remove(apolyline);
            }
        }

        void AddPolygons (IList polygons)
        {
            var map = NativeMap;
            if (map == null)
                return;

            if (_polygons == null)
                _polygons = new List<APolygon> ();

            _polygons.AddRange (polygons.Cast<Polygon> ().Select (polygon => {
                var opts = new PolygonOptions ();

                foreach (var p in polygon.Positions)
                    opts.Add (new LatLng (p.Latitude, p.Longitude));

                opts.InvokeStrokeWidth(polygon.StrokeWidth * _scaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps) 
                opts.InvokeStrokeColor(polygon.StrokeColor.ToAndroid());
                opts.InvokeFillColor(polygon.FillColor.ToAndroid());
                opts.Clickable (polygon.IsClickable);

                var nativePolygon = map.AddPolygon(opts);

                // associate pin with marker for later lookup in event handlers
                polygon.Id = nativePolygon;
                return nativePolygon;
            }));
        }

        void RemovePolygons (IList polygons)
        {
            var map = NativeMap;
            if (map == null)
                return;
            if (_polygons == null)
                return;

            foreach (Polygon polygon in polygons) {
                var apolygon = _polygons.FirstOrDefault (m => ((APolygon)polygon.Id).Id == m.Id);
                if (apolygon == null)
                    continue;
                apolygon.Remove();
                _polygons.Remove(apolygon);
            }
        }

        void AddCircles(IList circles)
        {
            var map = NativeMap;
            if (map == null)
                return;

            if (_circles == null)
                _circles = new List<ACircle>();

            _circles.AddRange(circles.Cast<Circle>().Select(circle =>
            {
                var opts = new CircleOptions();

                opts.InvokeCenter(new LatLng(circle.Center.Latitude, circle.Center.Longitude));
                opts.InvokeRadius(circle.Radius.Meters);
                opts.InvokeStrokeWidth(circle.StrokeWidth * _scaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps) 
                opts.InvokeStrokeColor(circle.StrokeColor.ToAndroid());
                opts.InvokeFillColor(circle.FillColor.ToAndroid());
                //opts.Clickable(circle.IsClickable);

                var nativeCircle = map.AddCircle(opts);

                // associate pin with marker for later lookup in event handlers
                circle.Id = nativeCircle;
                return nativeCircle;
            }));
        }

        void RemoveCircles(IList circles)
        {
            var map = NativeMap;
            if (map == null)
                return;
            if (_circles == null)
                return;

            foreach (Circle circle in circles)
            {
                var acircle = _circles.FirstOrDefault(m => ((ACircle)circle.Id).Id == m.Id);
                if (acircle == null)
                    continue;
                acircle.Remove();
                _circles.Remove(acircle);
            }
        }

		void AddTileLayers(IList tileLayers)
		{
			var map = NativeMap;
			if (map == null)
				return;

			if (_tileLayers == null)
				_tileLayers = new List<ATileOverlay>();

			_tileLayers.AddRange(tileLayers.Cast<ITileLayerInternal>().Select(tileLayer =>
			{
				var opts = new TileOverlayOptions();

				ITileProvider nativeTileProvider;

				if (tileLayer is UrlTileLayer)
				{
					nativeTileProvider = new NUrlTileLayer(((UrlTileLayer)tileLayer).MakeTileUri);
				}
				else 
				{
					// In future: For Non-UrlTileLayer. Now coding UrlTileLayer as dummy.  
					nativeTileProvider = new NUrlTileLayer(((UrlTileLayer)tileLayer).MakeTileUri);
				}
				var nativeTileOverlay = map.AddTileOverlay(opts.InvokeTileProvider(nativeTileProvider));

				// associate pin with marker for later lookup in event handlers
				tileLayer.Id = nativeTileOverlay;
				return nativeTileOverlay;
			}));
		}

		void RemoveTileLayers(IList tileLayers)
		{
			var map = NativeMap;
			if (map == null)
				return;
			if (_tileLayers == null)
				return;

			foreach (ITileLayerInternal tileLayer in tileLayers)
			{
				var atileLayer = _tileLayers.FirstOrDefault(m => ((ATileOverlay)tileLayer.Id).Id == m.Id);
				if (atileLayer == null)
					continue;
				atileLayer.Remove();
				_tileLayers.Remove(atileLayer);
			}
		}

        bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _disposed = true;

                var mapModel = Element as Map;
                if (mapModel != null)
                {
                    MessagingCenter.Unsubscribe<Map, MoveToRegionMessage>(this, MoveMessageName);
                    ((ObservableCollection<Pin>)mapModel.Pins).CollectionChanged -= OnPinCollectionChanged;
                    ((ObservableCollection<Polyline>)mapModel.Polylines).CollectionChanged -= OnPolylineCollectionChanged;
                    ((ObservableCollection<Polygon>)mapModel.Polygons).CollectionChanged -= OnPolygonCollectionChanged;
                    ((ObservableCollection<Circle>)mapModel.Circles).CollectionChanged -= OnCircleCollectionChanged;
					((ObservableCollection<ITileLayer>)mapModel.TileLayers).CollectionChanged -= OnCircleCollectionChanged;
                }

                var gmap = NativeMap;
                if (gmap == null)
                    return;
                gmap.MyLocationEnabled = false;
                gmap.InfoWindowClick -= MapOnInfoWindowClick;
                gmap.PolylineClick -= MapOnPolylineClick;
                gmap.PolygonClick -= MapOnPolygonClick;
                //gmap.CircleClick -= MapOnCircleClick;
                gmap.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
