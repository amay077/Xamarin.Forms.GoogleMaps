using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Xamarin.Forms.GoogleMaps
{
    public class Circle : BindableObject
    {
        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(nameof(StrokeWidth) , typeof(float), typeof(float), 1f);
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(Color), Color.Blue);
        public static readonly BindableProperty FillColorProperty = BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(Color), Color.Blue);
        //public static readonly BindableProperty IsClickableProperty = BindableProperty.Create("IsClickable", typeof(bool), typeof(bool), false);

        public static readonly BindableProperty CenterProperty = BindableProperty.Create(nameof(Center), typeof(Position), typeof(Position), default(Position));
        public static readonly BindableProperty RadiusProperty = BindableProperty.Create(nameof(Radius), typeof(Distance), typeof(Distance), Distance.FromMeters(1));

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

        public Distance Radius
        {
            get { return (Distance)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public object Tag { get; set; }

        public object NativeObject { get; internal set; }

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

