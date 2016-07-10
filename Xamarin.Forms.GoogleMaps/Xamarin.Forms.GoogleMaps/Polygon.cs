﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Xamarin.Forms.GoogleMaps
{
    public class Polygon : BindableObject
    {
        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create ("StrokeWidth", typeof (float), typeof (float), 1f);
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create ("StrokeColor", typeof (Color), typeof (Color), Color.Blue);
        public static readonly BindableProperty FillColorProperty = BindableProperty.Create ("FillColor", typeof (Color), typeof (Color), Color.Blue);
        public static readonly BindableProperty IsClickableProperty = BindableProperty.Create ("IsClickable", typeof (bool), typeof (bool), false);

        private readonly ObservableCollection<Position> _positions = new ObservableCollection<Position> ();

        private Action<Polygon, NotifyCollectionChangedEventArgs> _positionsChangedHandler = null;

        public float StrokeWidth {
            get { return (float)GetValue (StrokeWidthProperty); }
            set { SetValue (StrokeWidthProperty, value); }
        }

        public Color StrokeColor {
            get { return (Color)GetValue (StrokeColorProperty); }
            set { SetValue (StrokeColorProperty, value); }
        }
        public Color FillColor {
            get { return (Color)GetValue (FillColorProperty); }
            set { SetValue (FillColorProperty, value); }
        }


        public bool IsClickable {
            get { return (bool)GetValue (IsClickableProperty); }
            set { SetValue (IsClickableProperty, value); }
        }

        public IList<Position> Positions {
            get { return _positions; }
        }

        public object Tag { get; set; }

        public object NativeObject { get; internal set; }

        public event EventHandler Clicked;

        public Polygon ()
        {
        }

        internal bool SendTap ()
        {
            EventHandler handler = Clicked;
            if (handler == null)
                return false;

            handler (this, EventArgs.Empty);
            return true;
        }

        internal void SetOnPositionsChanged(Action<Polygon, NotifyCollectionChangedEventArgs> handler)
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

