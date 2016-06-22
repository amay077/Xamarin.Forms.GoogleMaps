using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.GoogleMaps
{
    public class Polyline : BindableObject
    {
        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create("StrokeWidth", typeof(float), typeof(float), 1f);
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create("StrokeColor", typeof(Color), typeof(Color), Color.Blue);

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

        public IList<Position> Positions
        {
            get { return _positions; }
        }

        internal object Id { get; set; }

        public Polyline()
        {
        }
    }
}

