using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using Xamarin.Forms.GoogleMaps.Internals;
using Xamarin.Forms.GoogleMaps.Helpers;
using System.Threading.Tasks;
using Xamarin.Forms.GoogleMaps.Extensions;

namespace Xamarin.Forms.GoogleMaps
{
    public class Map : View, IEnumerable<Pin>
    {
        public static readonly BindableProperty MapTypeProperty = BindableProperty.Create(nameof(MapType), typeof(MapType), typeof(Map), default(MapType));

#pragma warning disable CS0618 // Type or member is obsolete
        public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create(nameof(IsShowingUser), typeof(bool), typeof(Map), default(bool));

        public static readonly BindableProperty MyLocationEnabledProperty = BindableProperty.Create(nameof(MyLocationEnabled), typeof(bool), typeof(Map), default(bool));

        public static readonly BindableProperty HasScrollEnabledProperty = BindableProperty.Create(nameof(HasScrollEnabled), typeof(bool), typeof(Map), true);

        public static readonly BindableProperty HasZoomEnabledProperty = BindableProperty.Create(nameof(HasZoomEnabled), typeof(bool), typeof(Map), true);

        public static readonly BindableProperty HasRotationEnabledProperty = BindableProperty.Create(nameof(HasRotationEnabled), typeof(bool), typeof(Map), true);
#pragma warning restore CS0618 // Type or member is obsolete

        public static readonly BindableProperty SelectedPinProperty = BindableProperty.Create(nameof(SelectedPin), typeof(Pin), typeof(Map), default(Pin), defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty IsTrafficEnabledProperty = BindableProperty.Create(nameof(IsTrafficEnabled), typeof(bool), typeof(Map), false);

        public static readonly BindableProperty IndoorEnabledProperty = BindableProperty.Create(nameof(IsIndoorEnabled), typeof(bool), typeof(Map), true);

        public static readonly BindableProperty InitialCameraUpdateProperty = BindableProperty.Create(
            nameof(InitialCameraUpdate), typeof(CameraUpdate), typeof(Map),
            CameraUpdateFactory.NewPositionZoom(new Position(41.89, 12.49), 10),  // center on Rome by default
            propertyChanged: (bindable, oldValue, newValue) => 
            {
                ((Map)bindable)._useMoveToRegisonAsInitialBounds = false;   
            });

        public static readonly BindableProperty PaddingProperty = BindableProperty.Create(nameof(PaddingProperty), typeof(Thickness), typeof(Map), default(Thickness));

        bool _useMoveToRegisonAsInitialBounds = true;

        public static readonly BindableProperty CameraPositionProperty = BindableProperty.Create(
            nameof(CameraPosition), typeof(CameraPosition), typeof(Map),
            defaultValueCreator: (bindable) => new CameraPosition(((Map)bindable).InitialCameraUpdate.Position, 10),
            defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty MapStyleProperty = BindableProperty.Create(nameof(MapStyle), typeof(MapStyle), typeof(Map), null);

        readonly ObservableCollection<Pin> _pins = new ObservableCollection<Pin>();
        readonly ObservableCollection<Polyline> _polylines = new ObservableCollection<Polyline>();
        readonly ObservableCollection<Polygon> _polygons = new ObservableCollection<Polygon>();
        readonly ObservableCollection<Circle> _circles = new ObservableCollection<Circle>();
        readonly ObservableCollection<TileLayer> _tileLayers = new ObservableCollection<TileLayer>();
        readonly ObservableCollection<GroundOverlay> _groundOverlays = new ObservableCollection<GroundOverlay>();

        public event EventHandler<PinClickedEventArgs> PinClicked;
        public event EventHandler<SelectedPinChangedEventArgs> SelectedPinChanged;
        public event EventHandler<InfoWindowClickedEventArgs> InfoWindowClicked;
        public event EventHandler<InfoWindowLongClickedEventArgs> InfoWindowLongClicked;

        public event EventHandler<PinDragEventArgs> PinDragStart;
        public event EventHandler<PinDragEventArgs> PinDragEnd;
        public event EventHandler<PinDragEventArgs> PinDragging;

        public event EventHandler<MapClickedEventArgs> MapClicked;
        public event EventHandler<MapLongClickedEventArgs> MapLongClicked;
        public event EventHandler<MyLocationButtonClickedEventArgs> MyLocationButtonClicked;

        [Obsolete("Please use Map.CameraIdled instead of this")]
        public event EventHandler<CameraChangedEventArgs> CameraChanged;
        public event EventHandler<CameraMoveStartedEventArgs> CameraMoveStarted;
        public event EventHandler<CameraMovingEventArgs> CameraMoving;
        public event EventHandler<CameraIdledEventArgs> CameraIdled;

        internal Action<MoveToRegionMessage> OnMoveToRegion { get; set; }

        internal Action<CameraUpdateMessage> OnMoveCamera { get; set; }

        internal Action<CameraUpdateMessage> OnAnimateCamera { get; set; }

        internal Action<TakeSnapshotMessage> OnSnapshot{ get; set; }

        MapSpan _visibleRegion;
        MapRegion _region;

        //// Simone Marra
        //public static Position _TopLeft = new Position();
        //public static Position _TopRight = new Position();
        //public static Position _BottomLeft = new Position();
        //public static Position _BottomRight = new Position();
        //// End Simone Marra

        public Map()
        {
            VerticalOptions = HorizontalOptions = LayoutOptions.FillAndExpand;

            _pins.CollectionChanged += PinsOnCollectionChanged;
            _polylines.CollectionChanged += PolylinesOnCollectionChanged;
            _polygons.CollectionChanged += PolygonsOnCollectionChanged;
            _circles.CollectionChanged += CirclesOnCollectionChanged;
            _tileLayers.CollectionChanged += TileLayersOnCollectionChanged;
            _groundOverlays.CollectionChanged += GroundOverlays_CollectionChanged;
        }

        [Obsolete("Please use Map.UiSettings.ScrollGesturesEnabled instead of this")]
        public bool HasScrollEnabled
        {
            get { return (bool)GetValue(HasScrollEnabledProperty); }
            set { SetValue(HasScrollEnabledProperty, value); }
        }

        [Obsolete("Please use Map.UiSettings.ZoomGesturesEnabled and ZoomControlsEnabled instead of this")]
        public bool HasZoomEnabled
        {
            get { return (bool)GetValue(HasZoomEnabledProperty); }
            set { SetValue(HasZoomEnabledProperty, value); }
        }

        [Obsolete("Please use Map.UiSettings.RotateGesturesEnabled instead of this")]
        public bool HasRotationEnabled
        {
            get { return (bool)GetValue(HasRotationEnabledProperty); }
            set { SetValue(HasRotationEnabledProperty, value); }
        }

        public bool IsTrafficEnabled
        {
            get { return (bool)GetValue(IsTrafficEnabledProperty); }
            set { SetValue(IsTrafficEnabledProperty, value); }
        }

        public bool IsIndoorEnabled
        {
            get { return (bool) GetValue(IndoorEnabledProperty); }
            set { SetValue(IndoorEnabledProperty, value);}
        }

        [Obsolete("Please use Map.MyLocationEnabled and Map.UiSettings.MyLocationButtonEnabled instead of this")]
        public bool IsShowingUser
        {
            get { return (bool)GetValue(IsShowingUserProperty); }
            set { SetValue(IsShowingUserProperty, value); }
        }

        public bool MyLocationEnabled
        {
            get { return (bool)GetValue(MyLocationEnabledProperty); }
            set { SetValue(MyLocationEnabledProperty, value); }
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

        [TypeConverter(typeof(CameraUpdateConverter))]
        public CameraUpdate InitialCameraUpdate
        {
            get { return (CameraUpdate)GetValue(InitialCameraUpdateProperty); }
            set { SetValue(InitialCameraUpdateProperty, value); }
        }

        public CameraPosition CameraPosition
        {
            get { return (CameraPosition)GetValue(CameraPositionProperty); }
            internal set { SetValue(CameraPositionProperty, value); }
        }

        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        public MapStyle MapStyle
        {
            get { return (MapStyle)GetValue(MapStyleProperty); }
            set { SetValue(MapStyleProperty, value); }
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

        [Obsolete("Please use Map.Region instead of this")]
        public MapSpan VisibleRegion
        {
            get { return _visibleRegion; }
            internal set
            {
                if (_visibleRegion == value)
                    return;
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                OnPropertyChanging();
                _visibleRegion = value;
                OnPropertyChanged();
            }
        }

        public MapRegion Region
        {
            get { return _region; }
            internal set
            {
                if (_region == value)
                    return;
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                OnPropertyChanging();
                _region = value;
                OnPropertyChanged();
            }
        }

        public UiSettings UiSettings { get; } = new UiSettings();

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

            if (_useMoveToRegisonAsInitialBounds)
            {
                InitialCameraUpdate = CameraUpdateFactory.NewBounds(mapSpan.ToBounds(), 0);
                _useMoveToRegisonAsInitialBounds = false;
            }

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


        public Task<Stream> TakeSnapshot()
        {
            var comp = new TaskCompletionSource<Stream>();

            SendTakeSnapshot(new TakeSnapshotMessage(image => comp.SetResult(image)));

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

        internal void SendInfoWindowLongClicked(Pin pin)
        {
            var args = new InfoWindowLongClickedEventArgs(pin);
            InfoWindowLongClicked?.Invoke(this, args);
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

        internal void SendCameraMoveStarted(bool isGesture)
        {
            CameraMoveStarted?.Invoke(this, new CameraMoveStartedEventArgs(isGesture));
        }

        internal void SendCameraMoving(CameraPosition position)
        {
            CameraMoving?.Invoke(this, new CameraMovingEventArgs(position));
        }

        internal void SendCameraIdled(CameraPosition position)
        {
            CameraIdled?.Invoke(this, new CameraIdledEventArgs(position));
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

        void SendTakeSnapshot(TakeSnapshotMessage message)
        {
            OnSnapshot?.Invoke(message);
        }
    }
}
