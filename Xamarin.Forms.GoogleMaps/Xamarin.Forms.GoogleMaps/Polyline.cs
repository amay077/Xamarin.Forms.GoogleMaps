using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Xamarin.Forms.GoogleMaps
{
    public sealed class Polyline : BindableObject
    {
        void HandleAction(GoogleMaps.Polygon arg1, NotifyCollectionChangedEventArgs arg2)
        {

        }

        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(nameof(StrokeWidth), typeof(float), typeof(Polyline), 1f);
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(Polyline), Color.Blue);
        public static readonly BindableProperty IsClickableProperty = BindableProperty.Create(nameof(IsClickable), typeof(bool), typeof(Polyline), false);
        public static readonly BindableProperty ZIndexProperty = BindableProperty.Create(nameof(ZIndex), typeof(int), typeof(Polyline), 0);

        private readonly ObservableCollection<Position> _positions = new ObservableCollection<Position>();

        private Action<Polyline, NotifyCollectionChangedEventArgs> _positionsChangedHandler = null;

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

        public bool IsClickable
        {
            get { return (bool)GetValue(IsClickableProperty); }
            set { SetValue(IsClickableProperty, value); }
        }

        public int ZIndex
        {
            get { return (int)GetValue(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        public IList<Position> Positions
        {
            get { return _positions; }
        }

        public object Tag { get; set; }

        public object NativeObject { get; internal set; }

        public event EventHandler Clicked;

        public Polyline()
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

        internal void SetOnPositionsChanged(Action<Polyline, NotifyCollectionChangedEventArgs> handler)
        {
            _positionsChangedHandler = handler;
            if (handler != null)
                _positions.CollectionChanged += OnCollectionChanged;
            else
                _positions.CollectionChanged -= OnCollectionChanged;
        }

        void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _positionsChangedHandler?.Invoke(this, e);
        }
    }
}

