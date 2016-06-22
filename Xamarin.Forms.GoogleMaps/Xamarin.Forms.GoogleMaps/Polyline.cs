using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.GoogleMaps
{
    public class Polyline : BindableObject
    {
        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create("StrokeWidth", typeof(float), typeof(float), 1f);
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create("StrokeColor", typeof(Color), typeof(Color), Color.Blue);
        public static readonly BindableProperty IsClickableProperty = BindableProperty.Create("IsClickable", typeof(bool), typeof(bool), false);

        private readonly ObservableCollection<Position> _positions = new ObservableCollection<Position>();

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

        public IList<Position> Positions
        {
            get { return _positions; }
        }

        internal object Id { get; set; }

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
    }
}

