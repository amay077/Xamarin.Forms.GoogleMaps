namespace Xamarin.Forms.GoogleMaps
{
    public class UiSettings : BindableObject
    {
        public static readonly BindableProperty IsCompassEnabledProperty = BindableProperty.Create("IsCompassEnabled", typeof(bool), typeof(Map), true);
        public static readonly BindableProperty IsMyLocationButtonEnabledProperty = BindableProperty.Create("IsMyLocationButtonEnabled", typeof(bool), typeof(Map), true);
        public static readonly BindableProperty HasRotateEnabledProperty = BindableProperty.Create("HasRotateGesturesEnabled", typeof(bool), typeof(Map), true);
        public static readonly BindableProperty HasScrollEnabledProperty = BindableProperty.Create("HasScrollGesturesEnabled", typeof(bool), typeof(Map), true);
        public static readonly BindableProperty HasTiltEnabledProperty = BindableProperty.Create("HasTiltGesturesEnabled", typeof(bool), typeof(Map), true);
        public static readonly BindableProperty HasZoomEnabledProperty = BindableProperty.Create("HasZoomGesturesEnabled", typeof(bool), typeof(Map), true);

        //Android only
        public static readonly BindableProperty IsIndoorPickerEnabledProperty = BindableProperty.Create("IsIndoorPickerEnabled", typeof(bool), typeof(Map), true);
        public static readonly BindableProperty IsMapToolbarEnabledProperty = BindableProperty.Create("IsMapToolbarEnabled", typeof(bool), typeof(Map), true);

        //iOS only
        public static readonly BindableProperty ConsumesGesturesInViewProperty = BindableProperty.Create("ConsumesGesturesInView", typeof(bool), typeof(Map), true);
        public static readonly BindableProperty AllowScrollGesturesDuringRotateOrZoomProperty = BindableProperty.Create("AllowScrollDuringRotateOrZoom", typeof(bool), typeof(Map), true);


        public bool IsCompassEnabled
        {
            get { return (bool) GetValue(IsCompassEnabledProperty); }
            set { SetValue(IsCompassEnabledProperty, value);}
        }

        public bool IsIndoorPickerEnabled
        {
            get { return (bool)GetValue(IsIndoorPickerEnabledProperty); }
            set { SetValue(IsIndoorPickerEnabledProperty, value); }
        }
        public bool IsMapToolbarEnabled
        {
            get { return (bool)GetValue(IsMapToolbarEnabledProperty); }
            set { SetValue(IsMapToolbarEnabledProperty, value); }
        }

        public bool IsMyLocationButtonEnabled
        {
            get { return (bool)GetValue(IsMyLocationButtonEnabledProperty); }
            set { SetValue(IsMyLocationButtonEnabledProperty, value); }
        }

        public bool HasRotateEnabled
        {
            get { return (bool)GetValue(HasRotateEnabledProperty); }
            set { SetValue(HasRotateEnabledProperty, value); }
        }

        public bool HasScrollEnabled
        {
            get { return (bool)GetValue(HasScrollEnabledProperty); }
            set { SetValue(HasScrollEnabledProperty, value); }
        }

        public bool HasTiltEnabled
        {
            get { return (bool)GetValue(HasTiltEnabledProperty); }
            set { SetValue(HasTiltEnabledProperty, value); }
        }

        public bool HasZoomEnabled
        {
            get { return (bool)GetValue(HasZoomEnabledProperty); }
            set { SetValue(HasZoomEnabledProperty, value); }
        }

        public bool AllowScrollDuringRotateOrZoom
        {
            get { return (bool)GetValue(AllowScrollGesturesDuringRotateOrZoomProperty); }
            set { SetValue(AllowScrollGesturesDuringRotateOrZoomProperty, value); }
        }

        public bool ConsumesGesturesInView
        {
            get { return (bool)GetValue(ConsumesGesturesInViewProperty); }
            set { SetValue(ConsumesGesturesInViewProperty, value); }
        }
    }
}