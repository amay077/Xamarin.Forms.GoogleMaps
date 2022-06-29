using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Maui.GoogleMaps
{
    public sealed class Polygon : BindableObject
    {
        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(nameof(StrokeWidth), typeof(float), typeof(Polygon), 1f);
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(Polygon), Colors.Blue);
        public static readonly BindableProperty FillColorProperty = BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(Polygon), Colors.Blue);
        public static readonly BindableProperty IsClickableProperty = BindableProperty.Create(nameof(IsClickable), typeof(bool), typeof(Polygon), false);
        public static readonly BindableProperty ZIndexProperty = BindableProperty.Create(nameof(ZIndex), typeof(int), typeof(Polygon), 0);

        private readonly ObservableCollection<Position> _positions = new ObservableCollection<Position>();
        private readonly ObservableCollection<Position[]> _holes = new ObservableCollection<Position[]>();

        private Action<Polygon, NotifyCollectionChangedEventArgs> _positionsChangedHandler = null;
        private Action<Polygon, NotifyCollectionChangedEventArgs> _holesChangedHandler = null;

        public float StrokeWidth
        {
            get { return (float)GetValue(StrokeWidthProperty); }
            set { SetValue(StrokeWidthProperty, value); }
        }

        public Color StrokeColor
        {
            get { return (Color)GetValue(StrokeColorProperty); }
            set { SetValue(StrokeColorProperty, value); }
        }
        public Color FillColor
        {
            get { return (Color)GetValue(FillColorProperty); }
            set { SetValue(FillColorProperty, value); }
        }


        public bool IsClickable
        {
            get { return (bool)GetValue(IsClickableProperty); }
            set { SetValue(IsClickableProperty, value); }
        }

        public int ZIndex
        {
            get { return (int) GetValue(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        public IList<Position[]> Holes
        {
            get { return _holes; }
        }

        public IList<Position> Positions
        {
            get { return _positions; }
        }

        public object Tag { get; set; }

        public object NativeObject { get; internal set; }

        public event EventHandler Clicked;

        public Polygon()
        {
        }

        internal bool SendTap()
        {
            EventHandler handler = Clicked;
            if (handler == null)
                return false;

            handler(this, EventArgs.Empty);
            return true;
        }

        internal void SetOnPositionsChanged(Action<Polygon, NotifyCollectionChangedEventArgs> handler)
        {
            _positionsChangedHandler = handler;
            if (handler != null)
            {
                _positions.CollectionChanged += OnPositionsChanged;
            }
            else
            {
                _positions.CollectionChanged -= OnPositionsChanged;
            }
        }

        internal void SetOnHolesChanged(Action<Polygon, NotifyCollectionChangedEventArgs> handler)
        {
            _holesChangedHandler = handler;
            if (handler != null)
            {
                _holes.CollectionChanged += OnHolesChanged;
            }
            else
            {
                _holes.CollectionChanged -= OnHolesChanged;
            }
        }

        void OnPositionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _positionsChangedHandler?.Invoke(this, e);
        }

        void OnHolesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _holesChangedHandler?.Invoke(this, e);
        }
    }
}

