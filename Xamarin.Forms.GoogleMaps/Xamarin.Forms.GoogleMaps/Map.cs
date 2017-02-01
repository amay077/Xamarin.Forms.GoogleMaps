using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms.GoogleMaps.Internals;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms.GoogleMaps.Events;

namespace Xamarin.Forms.GoogleMaps
{
    public class Map : View, IEnumerable<Pin>
    {
        public static readonly BindableProperty MapTypeProperty = BindableProperty.Create("MapType", typeof(MapType), typeof(Map), default(MapType));

        public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create("IsShowingUser", typeof(bool), typeof(Map), default(bool));

        public static readonly BindableProperty HasScrollEnabledProperty = BindableProperty.Create("HasScrollEnabled", typeof(bool), typeof(Map), true);

        public static readonly BindableProperty HasZoomEnabledProperty = BindableProperty.Create("HasZoomEnabled", typeof(bool), typeof(Map), true);

        public static readonly BindableProperty SelectedPinProperty = BindableProperty.Create("SelectedPin", typeof(Pin), typeof(Map), default(Pin), defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty IsTrafficEnabledProperty = BindableProperty.Create("IsTrafficEnabled", typeof(bool), typeof(Map), false);

        readonly ObservableCollection<Pin> _pins = new ObservableCollection<Pin>();
        readonly ObservableCollection<Polyline> _polylines = new ObservableCollection<Polyline>();
        readonly ObservableCollection<Polygon> _polygons = new ObservableCollection<Polygon>();
        readonly ObservableCollection<Circle> _circles = new ObservableCollection<Circle>();
        readonly ObservableCollection<TileLayer> _tileLayers = new ObservableCollection<TileLayer>();
        readonly ObservableCollection<GroundOverlay> _groundOverlays = new ObservableCollection<GroundOverlay>();

        public event EventHandler<PinClickedEventArgs> PinClicked;
        public event EventHandler<SelectedPinChangedEventArgs> SelectedPinChanged;
        public event EventHandler<InfoWindowClickedEventArgs> InfoWindowClicked;

        public event EventHandler<PinDragEventArgs> PinDragStart;
        public event EventHandler<PinDragEventArgs> PinDragEnd;
        public event EventHandler<PinDragEventArgs> PinDragging;

        public event EventHandler<MapClickedEventArgs> MapClicked;
        public event EventHandler<MapLongClickedEventArgs> MapLongClicked;
        public event EventHandler<MyLocationButtonClickedEventArgs> MyLocationButtonClicked;
        public event EventHandler<CameraChangedEventArgs> CameraChanged;
        public event EventHandler<MapMoveEndedEventArgs> MapMoveEnded;

        internal Action<MoveToRegionMessage> OnMoveToRegion { get; set; }

        internal Action<CameraUpdateMessage> OnMoveCamera { get; set; }

        internal Action<CameraUpdateMessage> OnAnimateCamera { get; set; }

        MapSpan _visibleRegion;

        public Map(MapSpan region)
        {
            LastMoveToRegion = region;

            VerticalOptions = HorizontalOptions = LayoutOptions.FillAndExpand;

            _pins.CollectionChanged += PinsOnCollectionChanged;
            _polylines.CollectionChanged += PolylinesOnCollectionChanged;
            _polygons.CollectionChanged += PolygonsOnCollectionChanged;
            _circles.CollectionChanged += CirclesOnCollectionChanged;
            _tileLayers.CollectionChanged += TileLayersOnCollectionChanged;
            _groundOverlays.CollectionChanged += GroundOverlays_CollectionChanged;
        }

        // center on Rome by default
        public Map() : this(new MapSpan(new Position(41.890202, 12.492049), 0.1, 0.1))
        {
        }

        #region MapRegion
        // Allows to set map center at initiation
        public static readonly BindableProperty MapRegionProperty = BindableProperty.Create(nameof(VisibleRegion), typeof(MapSpan), typeof(Map), null, propertyChanged: OnMapRegionChanged);
        public MapSpan VisibleRegion { get { return (MapSpan)GetValue(MapRegionProperty); } set { SetValue(MapRegionProperty, value); } }
        public static readonly BindableProperty CameraMovingProperty = BindableProperty.Create(nameof(CameraMoving), typeof(bool), typeof(Map), false);
        public bool CameraMoving { get { return (bool)GetValue(CameraMovingProperty); } set { SetValue(CameraMovingProperty, value); } }

        internal void SetMapRegionInternal(MapSpan newValue)
        {
            CameraMoving = true;
            VisibleRegion = newValue;
            CameraMoving = false;
            SendMapMoveEnded();
        }

        private static void OnMapRegionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == oldValue) return;
            var newVal = newValue as MapSpan;
            if (newVal == null)
                return;
            var map = bindable as Map;
            if (map == null) return;

            if (map.CameraMoving)
            {
                map.MapRegionChanged?.Invoke(map, new Events.RegionEventArgs() { OldValue = oldValue as MapSpan, NewValue = newVal });
                return;
            }

            map.MoveToRegion(newVal, true);
        }

        public event EventHandler<RegionEventArgs> MapRegionChanged;

        #endregion MapRegion

        public bool HasScrollEnabled
        {
            get { return (bool)GetValue(HasScrollEnabledProperty); }
            set { SetValue(HasScrollEnabledProperty, value); }
        }

        public bool HasZoomEnabled
        {
            get { return (bool)GetValue(HasZoomEnabledProperty); }
            set { SetValue(HasZoomEnabledProperty, value); }
        }

        public bool IsTrafficEnabled
        {
            get { return (bool)GetValue(IsTrafficEnabledProperty); }
            set { SetValue(IsTrafficEnabledProperty, value); }
        }


        public bool IsShowingUser
        {
            get { return (bool)GetValue(IsShowingUserProperty); }
            set { SetValue(IsShowingUserProperty, value); }
        }

        public MapType MapType
        {
            get { return (MapType)GetValue(MapTypeProperty); }
            set { SetValue(MapTypeProperty, value); }
        }

        public Pin SelectedPin
        {
            get { return (Pin)GetValue(SelectedPinProperty); }
            set { SetValue(SelectedPinProperty, value); }
        }

        public IList<Pin> Pins
        {
            get { return _pins; }
        }

        public IList<Polyline> Polylines
        {
            get { return _polylines; }
        }

        public IList<Polygon> Polygons
        {
            get { return _polygons; }
        }

        public IList<Circle> Circles
        {
            get { return _circles; }
        }

        public IList<TileLayer> TileLayers
        {
            get { return _tileLayers; }
        }

        public IList<GroundOverlay> GroundOverlays
        {
            get { return _groundOverlays; }
        }

        internal MapSpan LastMoveToRegion { get; private set; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Pin> GetEnumerator()
        {
            return _pins.GetEnumerator();
        }

        public void MoveToRegion(MapSpan mapSpan, bool animate = true)
        {
            if (mapSpan == null)
                throw new ArgumentNullException(nameof(mapSpan));
            LastMoveToRegion = mapSpan;
            SendMoveToRegion(new MoveToRegionMessage(mapSpan, animate));
        }

        public Task<AnimationStatus> MoveCamera(CameraUpdate cameraUpdate)
        {
            var comp = new TaskCompletionSource<AnimationStatus>();

            SendMoveCamera(new CameraUpdateMessage(cameraUpdate, null, new DelegateAnimationCallback(
                () => comp.SetResult(AnimationStatus.Finished),
                () => comp.SetResult(AnimationStatus.Canceled))));

            return comp.Task;
        }

        public Task<AnimationStatus> AnimateCamera(CameraUpdate cameraUpdate, TimeSpan? duration = null)
        {
            var comp = new TaskCompletionSource<AnimationStatus>();

            SendAnimateCamera(new CameraUpdateMessage(cameraUpdate, duration, new DelegateAnimationCallback(
                () => comp.SetResult(AnimationStatus.Finished),
                () => comp.SetResult(AnimationStatus.Canceled))));

            return comp.Task;
        }

        void PinsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Cast<Pin>().Any(pin => pin.Label == null))
                throw new ArgumentException("Pin must have a Label to be added to a map");
        }

        void PolylinesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Cast<Polyline>().Any(polyline => polyline.Positions.Count < 2))
                throw new ArgumentException("Polyline must have a 2 positions to be added to a map");
        }

        void PolygonsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Cast<Polygon>().Any(polygon => polygon.Positions.Count < 3))
                throw new ArgumentException("Polygon must have a 3 positions to be added to a map");
        }

        void CirclesOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Cast<Circle>().Any(circle => (
                circle?.Center == null || circle?.Radius == null || circle.Radius.Meters <= 0f)))
                throw new ArgumentException("Circle must have a center and radius");
        }

        void TileLayersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            //if (e.NewItems != null && e.NewItems.Cast<ITileLayer>().Any(tileLayer => (circle.Center == null || circle.Radius == null || circle.Radius.Meters <= 0f)))
            //  throw new ArgumentException("Circle must have a center and radius");
        }

        void GroundOverlays_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        internal void SendSelectedPinChanged(Pin selectedPin)
        {
            SelectedPinChanged?.Invoke(this, new SelectedPinChangedEventArgs(selectedPin));
        }

        internal bool SendPinClicked(Pin pin)
        {
            var args = new PinClickedEventArgs(pin);
            PinClicked?.Invoke(this, args);
            return args.Handled;
        }

        internal void SendInfoWindowClicked(Pin pin)
        {
            var args = new InfoWindowClickedEventArgs(pin);
            InfoWindowClicked?.Invoke(this, args);
        }

        internal void SendPinDragStart(Pin pin)
        {
            PinDragStart?.Invoke(this, new PinDragEventArgs(pin));
        }

        internal void SendPinDragEnd(Pin pin)
        {
            PinDragEnd?.Invoke(this, new PinDragEventArgs(pin));
        }

        internal void SendPinDragging(Pin pin)
        {
            PinDragging?.Invoke(this, new PinDragEventArgs(pin));
        }

        internal void SendMapClicked(Position point)
        {
            MapClicked?.Invoke(this, new MapClickedEventArgs(point));
        }

        internal void SendMapLongClicked(Position point)
        {
            MapLongClicked?.Invoke(this, new MapLongClickedEventArgs(point));
        }

        internal bool SendMyLocationClicked()
        {
            var args = new MyLocationButtonClickedEventArgs();
            MyLocationButtonClicked?.Invoke(this, args);
            return args.Handled;
        }

        internal void SendCameraChanged(CameraPosition position)
        {
            CameraChanged?.Invoke(this, new CameraChangedEventArgs(position));
        }

        private void SendMoveToRegion(MoveToRegionMessage message)
        {
            OnMoveToRegion?.Invoke(message);
        }

        void SendMoveCamera(CameraUpdateMessage message)
        {
            OnMoveCamera?.Invoke(message);
        }

        void SendAnimateCamera(CameraUpdateMessage message)
        {
            OnAnimateCamera?.Invoke(message);
        }

        internal void SendMapMoveEnded()
        {
            var args = new MapMoveEndedEventArgs() { Region = VisibleRegion };
            MapMoveEnded?.Invoke(this, args);
        }
    }
}
