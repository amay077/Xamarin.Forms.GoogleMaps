using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.GoogleMaps
{
    public class Circle : BindableObject
    {
        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create("StrokeWidth", typeof(float), typeof(float), 1f);
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create("StrokeColor", typeof(Color), typeof(Color), Color.Blue);
        public static readonly BindableProperty FillColorProperty = BindableProperty.Create("FillColor", typeof(Color), typeof(Color), Color.Blue);
        //public static readonly BindableProperty IsClickableProperty = BindableProperty.Create("IsClickable", typeof(bool), typeof(bool), false);

        public static readonly BindableProperty CenterProperty = BindableProperty.Create("Center", typeof(Position), typeof(Position), default(Position));
        public static readonly BindableProperty RadiusProperty = BindableProperty.Create("Radius", typeof(float), typeof(float), 1f);

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

        //public bool IsClickable
        //{
        //    get { return (bool)GetValue(IsClickableProperty); }
        //    set { SetValue(IsClickableProperty, value); }
        //}

        public Position Center
        {
            get { return (Position)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        public float Radius
        {
            get { return (float)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        internal object Id { get; set; }

        //public event EventHandler Clicked;

        public Circle()
        {
        }

        internal bool SendTap()
        {
            //EventHandler handler = Clicked;
            //if (handler == null)
            //    return false;

            //handler(this, EventArgs.Empty);
            //return true;
            return false;
        }
    }
}

