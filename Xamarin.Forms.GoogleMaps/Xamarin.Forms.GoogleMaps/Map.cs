using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms.GoogleMaps.Events;
using Xamarin.Forms.GoogleMaps.Internals;

namespace Xamarin.Forms.GoogleMaps
{
    public class Map : View, IEnumerable<Pin>
    {
        #region PinsSource Property
        public static readonly BindableProperty PinsSourceProperty = BindableProperty.Create(nameof(PinsSource), typeof(ObservableCollection<IPin>), typeof(Map), null, propertyChanged: OnPinsSourceChanged);
        public ObservableCollection<IPin> PinsSource { get { return (ObservableCollection<IPin>)GetValue(PinsSourceProperty); } set { SetValue(PinsSourceProperty, value); } }
        private void Register(ObservableCollection<IPin> source) { if (source != null) source.CollectionChanged += OnPinSourceCollectionChanged; }
        private void Unregister(ObservableCollection<IPin> source) { if (source != null) source.CollectionChanged -= OnPinSourceCollectionChanged; }

        static void OnPinsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var map = bindable as Map;
            if (map == null) return;
            var source = oldValue as ObservableCollection<IPin>;
            if (source != null) map.Unregister(source);
            source = newValue as ObservableCollection<IPin>;
            if (source != null) map.Register(source);
        }

        void OnPinSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
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
                    var pins = Pins.ToList();
                    foreach (var p in pins) pins.Remove(p);
                    AddPins((IList)PinsSource);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
            }
        }

        private void RemovePins(IList oldPins)
        {
            if (oldPins != null)
                foreach (IPin oldPin in oldPins)
                {
                    var pin = Pins.FirstOrDefault(p => ReferenceEquals(oldPin, p.BindingContext));
                    Pins.Remove(pin);
                }
        }

        private void AddPins(IList newPins)
        {
            if (newPins != null)
                foreach (IPin p in newPins) Pins.Add(p.ToPin());
        }
        #endregion PinsSource

        #region CirclesSource Property
        public static readonly BindableProperty CirclesSourceProperty = BindableProperty.Create(nameof(CirclesSource), typeof(ObservableCollection<ICircle>), typeof(Map), null, propertyChanged: OnCirclesSourceChanged);
        public ObservableCollection<ICircle> CirclesSource { get { return (ObservableCollection<ICircle>)GetValue(CirclesSourceProperty); } set { SetValue(CirclesSourceProperty, value); } }
        private void Register(ObservableCollection<ICircle> source) { if (source != null) source.CollectionChanged += OnCircleSourceCollectionChanged; }
        private void Unregister(ObservableCollection<ICircle> source) { if (source != null) source.CollectionChanged -= OnCircleSourceCollectionChanged; }

        static void OnCirclesSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var map = bindable as Map;
            if (map == null) return;
            var source = oldValue as ObservableCollection<ICircle>;
            if (source != null) map.Unregister(source);
            source = newValue as ObservableCollection<ICircle>;
            if (source != null) map.Register(source);
        }

        void OnCircleSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
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
                    var circles = Circles.ToList();
                    foreach (var p in circles) circles.Remove(p);
                    AddCircles((IList)CirclesSource);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
            }
        }

        private void RemoveCircles(IList oldCircles)
        {
            if (oldCircles != null)
                foreach (ICircle oldCircle in oldCircles)
                {
                    var circle = Circles.FirstOrDefault(p => ReferenceEquals(oldCircle, p.BindingContext));
                    Circles.Remove(circle);
                }
        }

        private void AddCircles(IList newCircles)
        {
            if (newCircles != null)
                foreach (ICircle p in newCircles) Circles.Add(p.ToCircle());
        }
        #endregion CirclesSource

        #region MapRegion 
        // Allows to set map center at initiation
        public static readonly BindableProperty MapRegionProperty = BindableProperty.Create(nameof(MapRegion), typeof(MapSpan), typeof(Map), null, propertyChanged: OnMapRegionChanged);
        public MapSpan MapRegion { get { return (MapSpan)GetValue(MapRegionProperty); } set { SetValue(MapRegionProperty, value); } }
        public bool CameraMoving { get; set; }
        private static void OnMapRegionChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (newValue == oldValue) return;
            var newVal = newValue as MapSpan;
            if (newVal == null) throw new ArgumentNullException(nameof(newValue));
            var map = bindable as Map;
            if (map == null) return;
            if (map.CameraMoving)
            {
                map.MapRegionChanged?.Invoke(map, new Events.RegionEventArgs() { OldValue = oldValue as MapSpan, NewValue = newVal });
                return;
            }
            else
                map.MoveToRegion(newVal, true);
        }
        public event EventHandler<RegionEventArgs> MapRegionChanged;
        #endregion MapRegion

        public static readonly BindableProperty MapTypeProperty = BindableProperty.Create(nameof(MapType), typeof(MapType), typeof(Map), default(MapType));

        public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create(nameof(IsShowingUser), typeof(bool), typeof(Map), default(bool));

        public static readonly BindableProperty HasScrollEnabledProperty = BindableProperty.Create(nameof(HasScrollEnabled), typeof(bool), typeof(Map), true);

        public static readonly BindableProperty HasZoomEnabledProperty = BindableProperty.Create(nameof(HasZoomEnabled), typeof(bool), typeof(Map), true);

        public static readonly BindableProperty IsTrafficEnabledProperty = BindableProperty.Create(nameof(IsTrafficEnabled), typeof(bool), typeof(Map), false);

        public static readonly BindableProperty SelectedPinProperty = BindableProperty.Create(nameof(SelectedPin), typeof(Pin), typeof(Map), default(Pin), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedPinChanged);

        public static readonly BindableProperty SelectedItemProperty = BindableProperty.Create(nameof(SelectedItem), typeof(IPin), typeof(Map), default(IPin), defaultBindingMode: BindingMode.TwoWay, propertyChanged: OnSelectedItemChanged);

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var map = bindable as Map; if (map == null) return;
            if (newValue == null && map.SelectedPin != null) { map.SelectedPin = null; return; }
            var pin = map.Pins.FirstOrDefault(p => p.BindingContext == newValue);
            // could add an exception if null, it means the object is not in the list
            map.SelectedPin = pin;
        }

        private static void OnSelectedPinChanged(BindableObject bindable, object oldValue, object newValue)
        { var map = bindable as Map; if (map == null) return; map.SelectedItem = map.SelectedPin?.BindingContext as IPin; }

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

        public Map(MapSpan region)
        {
            LastMoveToRegion = MapRegion ?? region;

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

        public IPin SelectedItem
        {
            get { return (IPin)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
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
            if (LastMoveToRegion == mapSpan) return;
            LastMoveToRegion = mapSpan;
            MessagingCenter.Send(this, "MapMoveToRegion", new MoveToRegionMessage(mapSpan, animate));
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
            //	throw new ArgumentException("Circle must have a center and radius");
        }

        void GroundOverlays_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        public static readonly BindableProperty SelectedPinChangedCommandProperty = BindableProperty.Create(nameof(SelectedPinChangedCommand), typeof(ICommand), typeof(Map), null);
        public ICommand SelectedPinChangedCommand { get { return (ICommand)GetValue(SelectedPinChangedCommandProperty); } set { SetValue(SelectedPinChangedCommandProperty, value); } }

        public static readonly BindableProperty PinClickedCommandProperty = BindableProperty.Create(nameof(PinClickedCommand), typeof(ICommand), typeof(Map), null);
        public ICommand PinClickedCommand { get { return (ICommand)GetValue(PinClickedCommandProperty); } set { SetValue(PinClickedCommandProperty, value); } }

        public static readonly BindableProperty InfoWindowClickedCommandProperty = BindableProperty.Create(nameof(InfoWindowClickedCommand), typeof(ICommand), typeof(Map), null);
        public ICommand InfoWindowClickedCommand { get { return (ICommand)GetValue(InfoWindowClickedCommandProperty); } set { SetValue(InfoWindowClickedCommandProperty, value); } }

        public static readonly BindableProperty PinDragStartCommandProperty = BindableProperty.Create(nameof(PinDragStartCommand), typeof(ICommand), typeof(Map), null);
        public ICommand PinDragStartCommand { get { return (ICommand)GetValue(PinDragStartCommandProperty); } set { SetValue(PinDragStartCommandProperty, value); } }

        public static readonly BindableProperty PinDragEndCommandProperty = BindableProperty.Create(nameof(PinDragEndCommand), typeof(ICommand), typeof(Map), null);
        public ICommand PinDragEndCommand { get { return (ICommand)GetValue(PinDragEndCommandProperty); } set { SetValue(PinDragEndCommandProperty, value); } }

        public static readonly BindableProperty PinDraggingCommandProperty = BindableProperty.Create(nameof(PinDraggingCommand), typeof(ICommand), typeof(Map), null);
        public ICommand PinDraggingCommand { get { return (ICommand)GetValue(PinDraggingCommandProperty); } set { SetValue(PinDraggingCommandProperty, value); } }

        public static readonly BindableProperty MapClickedCommandProperty = BindableProperty.Create(nameof(MapClickedCommand), typeof(ICommand), typeof(Map), null);
        public ICommand MapClickedCommand { get { return (ICommand)GetValue(MapClickedCommandProperty); } set { SetValue(MapClickedCommandProperty, value); } }

        public static readonly BindableProperty MapLongClickedCommandProperty = BindableProperty.Create(nameof(MapLongClickedCommand), typeof(ICommand), typeof(Map), null);
        public ICommand MapLongClickedCommand { get { return (ICommand)GetValue(MapLongClickedCommandProperty); } set { SetValue(MapLongClickedCommandProperty, value); } }


        internal void SendSelectedPinChanged(Pin selectedPin)
        {
            SelectedPinChanged?.Invoke(this, new SelectedPinChangedEventArgs(selectedPin));
            if (SelectedPinChangedCommand?.CanExecute(selectedPin.BindingContext as IPin)??false) SelectedPinChangedCommand.Execute(selectedPin.BindingContext as IPin);
        }

        internal bool SendPinClicked(Pin pin)
        {
            var args = new PinClickedEventArgs(pin);
            PinClicked?.Invoke(this, args);
            if (PinClickedCommand?.CanExecute(pin.BindingContext as IPin) ?? false) PinClickedCommand.Execute(pin.BindingContext as IPin);
            return args.Handled;
        }

        internal void SendInfoWindowClicked(Pin pin)
        {
            var args = new InfoWindowClickedEventArgs(pin);
            InfoWindowClicked?.Invoke(this, args);
            if (InfoWindowClickedCommand?.CanExecute(pin.BindingContext as IPin) ?? false) InfoWindowClickedCommand.Execute(pin.BindingContext as IPin);
        }

        internal void SendPinDragStart(Pin pin)
        {
            PinDragStart?.Invoke(this, new PinDragEventArgs(pin));
            if (PinDragStartCommand?.CanExecute(pin.BindingContext as IPin) ?? false) PinDragStartCommand.Execute(pin.BindingContext as IPin);
        }

        internal void SendPinDragEnd(Pin pin)
        {
            PinDragEnd?.Invoke(this, new PinDragEventArgs(pin));
            if (PinDragEndCommand?.CanExecute(pin.BindingContext as IPin) ?? false) PinDragEndCommand.Execute(pin.BindingContext as IPin);
        }

        internal void SendPinDragging(Pin pin)
        {
            PinDragging?.Invoke(this, new PinDragEventArgs(pin));
            if (PinDraggingCommand?.CanExecute(pin.BindingContext as IPin) ?? false) PinDraggingCommand.Execute(pin.BindingContext as IPin);
        }

        internal void SendMapClicked(Position point)
        {
            MapClicked?.Invoke(this, new MapClickedEventArgs(point));
            if (MapClickedCommand?.CanExecute(point) ?? false) MapClickedCommand.Execute(point);
        }

        internal void SendMapLongClicked(Position point)
        {
            MapLongClicked?.Invoke(this, new MapLongClickedEventArgs(point));
            if (MapLongClickedCommand?.CanExecute(point) ?? false) MapLongClickedCommand.Execute(point);
        }
    }
}
