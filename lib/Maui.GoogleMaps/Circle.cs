
namespace Maui.GoogleMaps
{
    public sealed class Circle : BindableObject
    {
        public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create(nameof(StrokeWidth) , typeof(float), typeof(Circle), 1f);
        public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create(nameof(StrokeColor), typeof(Color), typeof(Circle), Colors.Blue);
        public static readonly BindableProperty FillColorProperty = BindableProperty.Create(nameof(FillColor), typeof(Color), typeof(Circle), Colors.Blue);
        public static readonly BindableProperty IsClickableProperty = BindableProperty.Create("IsClickable", typeof(bool), typeof(Circle), false);

        public static readonly BindableProperty CenterProperty = BindableProperty.Create(nameof(Center), typeof(Position), typeof(Circle), default(Position));
        public static readonly BindableProperty RadiusProperty = BindableProperty.Create(nameof(Radius), typeof(Distance), typeof(Circle), Distance.FromMeters(1));
        public static readonly BindableProperty ZIndexProperty = BindableProperty.Create(nameof(ZIndex), typeof(int), typeof(Circle), 0);

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

        public int ZIndex
        {
            get { return (int)GetValue(ZIndexProperty); }
            set { SetValue(ZIndexProperty, value); }
        }

        public object Tag { get; set; }

        public object NativeObject { get; internal set; }

        public event EventHandler Clicked;

        public Circle()
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

