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
using System.Collections;
using ATileOverlay = Android.Gms.Maps.Model.TileOverlay;
using Android.Util;
using Android.App;
using Xamarin.Forms.GoogleMaps.Internals;
using Xamarin.Forms.GoogleMaps.Android.Logics;

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

        //List<Marker> _markers;
		List<ATileOverlay> _tileLayers;

        PolylineLogic _polylineLogic = new PolylineLogic();
        PolygonLogic _polygonLogic = new PolygonLogic();
        CircleLogic _circleLogic = new CircleLogic();
        PinLogic _pinLogic = new PinLogic();
        float _scaledDensity = 1;

        const string MoveMessageName = "MapMoveToRegion";

#pragma warning disable 618
        protected GoogleMap NativeMap => ((MapView)Control).Map;
#pragma warning restore 618

        protected Map Map => (Map)Element;


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
                _polylineLogic.ScaledDensity = metrics.ScaledDensity;
                _polygonLogic.ScaledDensity = metrics.ScaledDensity;
                _circleLogic.ScaledDensity = metrics.ScaledDensity;
            }

            if (e.OldElement != null)
            {
                var oldMapModel = (Map)e.OldElement;
				((ObservableCollection<TileLayer>)oldMapModel.TileLayers).CollectionChanged -= OnTileLayerCollectionChanged;

                MessagingCenter.Unsubscribe<Map, MoveToRegionMessage>(this, MoveMessageName);

#pragma warning disable 618
                if (oldMapView.Map != null)
                {
#pragma warning restore 618

#pragma warning disable 618
                    oldMapView.Map.SetOnCameraChangeListener(null);
#pragma warning restore 618
                    //NativeMap.InfoWindowClick -= MapOnInfoWindowClick;
                    //NativeMap.PolylineClick -= MapOnPolylineClick;
                    //NativeMap.PolygonClick -= MapOnPolygonClick;
                    //NativeMap.CircleClick -= MapOnCircleClick; // Circle click is not supported.
                    //NativeMap.MarkerClick -= MapOnMakerClick;
                    //NativeMap.InfoWindowClose -= MapOnInfoWindowClose;
                }

                oldMapView.Dispose();
            }

            var map = NativeMap;
            if (map != null)
            {
                map.SetOnCameraChangeListener(this);
                //NativeMap.InfoWindowClick += MapOnInfoWindowClick;
                //NativeMap.PolygonClick += MapOnPolygonClick;
                //NativeMap.CircleClick += MapOnCircleClick; // Circle click is not supported.
                //NativeMap.MarkerClick += MapOnMakerClick;
                //NativeMap.InfoWindowClose += MapOnInfoWindowClose;

                map.UiSettings.ZoomControlsEnabled = Map.HasZoomEnabled;
                map.UiSettings.ZoomGesturesEnabled = Map.HasZoomEnabled;
                map.UiSettings.ScrollGesturesEnabled = Map.HasScrollEnabled;
                map.MyLocationEnabled = map.UiSettings.MyLocationButtonEnabled = Map.IsShowingUser;
                SetMapType();
            }

#pragma warning disable 618
            _polylineLogic.Register(oldMapView?.Map, (Map)e.OldElement, NativeMap, Map);
            _polygonLogic.Register(oldMapView?.Map, (Map)e.OldElement, NativeMap, Map);
            _circleLogic.Register(oldMapView?.Map, (Map)e.OldElement, NativeMap, Map);
            _pinLogic.Register(oldMapView?.Map, (Map)e.OldElement, NativeMap, Map);
#pragma warning restore 618

            MessagingCenter.Subscribe<Map, MoveToRegionMessage>(this, MoveMessageName, OnMoveToRegionMessage, Map);

			var inccTileLayer = Map.TileLayers as INotifyCollectionChanged;
			if (inccTileLayer != null)
				inccTileLayer.CollectionChanged += OnTileLayerCollectionChanged;
        }

        //void OnPinCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        //{
        //    switch (notifyCollectionChangedEventArgs.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:
        //            AddPins(notifyCollectionChangedEventArgs.NewItems);
        //            break;
        //        case NotifyCollectionChangedAction.Remove:
        //            RemovePins(notifyCollectionChangedEventArgs.OldItems);
        //            break;
        //        case NotifyCollectionChangedAction.Replace:
        //            RemovePins(notifyCollectionChangedEventArgs.OldItems);
        //            AddPins(notifyCollectionChangedEventArgs.NewItems);
        //            break;
        //        case NotifyCollectionChangedAction.Reset:
        //            _markers?.ForEach(m => m.Remove());
        //            _markers = null;
        //            ((Map)Element).SelectedPin = null;
        //            AddPins((IList)(Element as Map).Pins);
        //            break;
        //        case NotifyCollectionChangedAction.Move:
        //            //do nothing
        //            break;
        //    }
        //}

        //void OnPolygonCollectionChanged (object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        //{
        //    switch (notifyCollectionChangedEventArgs.Action) {
        //    case NotifyCollectionChangedAction.Add:
        //        AddPolygons (notifyCollectionChangedEventArgs.NewItems);
        //        break;
        //    case NotifyCollectionChangedAction.Remove:
        //        RemovePolygons (notifyCollectionChangedEventArgs.OldItems);
        //        break;
        //    case NotifyCollectionChangedAction.Replace:
        //        RemovePolygons (notifyCollectionChangedEventArgs.OldItems);
        //        AddPolygons (notifyCollectionChangedEventArgs.NewItems);
        //        break;
        //    case NotifyCollectionChangedAction.Reset:
        //        _polygons?.ForEach (polygon => polygon.Remove());
        //        _polygons = null;
        //        AddPolygons ((IList)(Element as Map).Polygons);
        //        break;
        //    case NotifyCollectionChangedAction.Move:
        //        //do nothing
        //        break;
        //    }
        //}

        //void OnCircleCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        //{
        //    switch (notifyCollectionChangedEventArgs.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:
        //            AddCircles(notifyCollectionChangedEventArgs.NewItems);
        //            break;
        //        case NotifyCollectionChangedAction.Remove:
        //            RemoveCircles(notifyCollectionChangedEventArgs.OldItems);
        //            break;
        //        case NotifyCollectionChangedAction.Replace:
        //            RemoveCircles(notifyCollectionChangedEventArgs.OldItems);
        //            AddCircles(notifyCollectionChangedEventArgs.NewItems);
        //            break;
        //        case NotifyCollectionChangedAction.Reset:
        //            _circles?.ForEach(circle => circle.Remove());
        //            _circles = null;
        //            AddCircles((IList)(Element as Map).Circles);
        //            break;
        //        case NotifyCollectionChangedAction.Move:
        //            //do nothing
        //            break;
        //    }
        //}

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
                _polylineLogic.NotifyReset();
                _polygonLogic.NotifyReset();
                _circleLogic.NotifyReset();
                _pinLogic.NotifyReset();
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

            _pinLogic.OnElementPropertyChanged(e);
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

        //void AddPins(IList pins)
        //{
        //    var map = NativeMap;
        //    if (map == null)
        //        return;

        //    if (_markers == null)
        //        _markers = new List<Marker>();

        //    _markers.AddRange(pins.Cast<Pin>().Select(p =>
        //    {
        //        var pin = (Pin)p;
        //        var opts = new MarkerOptions();
        //        opts.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
        //        opts.SetTitle(pin.Label);
        //        opts.SetSnippet(pin.Address);
        //        var marker = map.AddMarker(opts);

        //        // associate pin with marker for later lookup in event handlers
        //        pin.Id = marker;
        //        return marker;
        //    }));

        //    UpdateSelectedPin(Map.SelectedPin);
        //}



        //void RemovePins(IList pins)
        //{
        //    var gmap = (Map)Element;
        //    var map = NativeMap;
        //    if (map == null)
        //        return;
        //    if (_markers == null)
        //        return;

        //    foreach (Pin p in pins)
        //    {
        //        var marker = _markers.FirstOrDefault(m => ((Marker)p.Id).Id == m.Id);
        //        if (marker == null)
        //            continue;
        //        marker.Remove();
        //        _markers.Remove(marker);

        //        if (object.ReferenceEquals(gmap.SelectedPin,  p))
        //            gmap.SelectedPin = null;

        //    }
        //}

        //void MapOnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs eventArgs)
        //{
        //    // clicked marker
        //    var marker = eventArgs.Marker;

        //    // lookup pin
        //    Pin targetPin = null;
        //    for (var i = 0; i < Map.Pins.Count; i++)
        //    {
        //        var pin = Map.Pins[i];
        //        if (((Marker)pin.Id).Id != marker.Id)
        //            continue;

        //        targetPin = pin;
        //        break;
        //    }

        //    // only consider event handled if a handler is present. 
        //    // Else allow default behavior of displaying an info window.
        //    targetPin?.SendTap();
        //}

        //void MapOnMakerClick(object sender, GoogleMap.MarkerClickEventArgs eventArgs)
        //{
        //    var map = (Map)Element;

        //    // clicked marker
        //    var marker = eventArgs.Marker;

        //    // lookup pin
        //    Pin targetPin = null;
        //    for (var i = 0; i < Map.Pins.Count; i++)
        //    {
        //        var pin = Map.Pins[i];
        //        if (((Marker)pin.Id).Id != marker.Id)
        //            continue;

        //        targetPin = pin;
        //        break;
        //    }

        //    try
        //    {
        //        _onMarkerEvent = true;
        //        if (targetPin != null && !object.ReferenceEquals(targetPin, map.SelectedPin))
        //            map.SelectedPin = targetPin;
        //    }
        //    finally
        //    {
        //        _onMarkerEvent = false;
        //    }

        //    eventArgs.Handled = false;
        //}

        //void MapOnInfoWindowClose(object sender, GoogleMap.InfoWindowCloseEventArgs eventArgs)
        //{
        //    System.Diagnostics.Debug.WriteLine("MapOnInfoWindowClose");
        //    var map = (Map)Element;

        //    // clicked marker
        //    var marker = eventArgs.Marker;

        //    // lookup pin
        //    Pin targetPin = null;
        //    for (var i = 0; i < Map.Pins.Count; i++)
        //    {
        //        var pin = Map.Pins[i];
        //        if (((Marker)pin.Id).Id != marker.Id)
        //            continue;

        //        targetPin = pin;
        //        break;
        //    }

        //    try
        //    {
        //        _onMarkerEvent = true;
        //        if (targetPin != null && object.ReferenceEquals(targetPin, map.SelectedPin))
        //            map.SelectedPin = null;
        //    }
        //    finally
        //    {
        //        _onMarkerEvent = false;
        //    }
        //}

		void AddTileLayers(IList tileLayers)
		{
			var map = NativeMap;
			if (map == null)
				return;

			if (_tileLayers == null)
				_tileLayers = new List<ATileOverlay>();

			_tileLayers.AddRange(tileLayers.Cast<TileLayer>().Select(tileLayer =>
			{
				var opts = new TileOverlayOptions();

				ITileProvider nativeTileProvider;

				if (tileLayer.MakeTileUri != null)
				{
					nativeTileProvider = new NUrlTileLayer(tileLayer.MakeTileUri, tileLayer.TileSize);
				}
				else if (tileLayer.TileImageSync != null)
				{
					nativeTileProvider = new NSyncTileLayer(tileLayer.TileImageSync, tileLayer.TileSize);
				} 
				else 
				{ 
					nativeTileProvider = new NAsyncTileLayer(tileLayer.TileImageAsync, tileLayer.TileSize);
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

			foreach (TileLayer tileLayer in tileLayers)
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

                if (this.Map != null)
                {
                    MessagingCenter.Unsubscribe<Map, MoveToRegionMessage>(this, MoveMessageName);
                    //((ObservableCollection<Pin>)Map.Pins).CollectionChanged -= OnPinCollectionChanged;
                    //((ObservableCollection<Polyline>)Map.Polylines).CollectionChanged -= OnPolylineCollectionChanged;
                    //((ObservableCollection<Polygon>)Map.Polygons).CollectionChanged -= OnPolygonCollectionChanged;
                    //((ObservableCollection<Circle>)Map.Circles).CollectionChanged -= OnCircleCollectionChanged;
					((ObservableCollection<TileLayer>)Map.TileLayers).CollectionChanged -= OnTileLayerCollectionChanged;
                }

                _circleLogic.Unregister(NativeMap, Map);
                _polylineLogic.Unregister(NativeMap, Map);
                _polygonLogic.Unregister(NativeMap, Map);
                _pinLogic.Unregister(NativeMap, Map);


                if (NativeMap == null)
                    return;
                NativeMap.MyLocationEnabled = false;
                //NativeMap.InfoWindowClick -= MapOnInfoWindowClick;
                //gmap.PolylineClick -= MapOnPolylineClick;
                //NativeMap.PolygonClick -= MapOnPolygonClick;
                //gmap.CircleClick -= MapOnCircleClick;
                NativeMap.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
