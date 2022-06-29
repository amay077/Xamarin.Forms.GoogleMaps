
namespace Maui.GoogleMaps
{
    public sealed class UiSettings : BindableObject
    {
        public static readonly BindableProperty CompassEnabledEnabledProperty = BindableProperty.Create(
            nameof(CompassEnabled), typeof(bool), typeof(UiSettings), true);
        public bool CompassEnabled
        {
            get { return (bool)GetValue(CompassEnabledEnabledProperty); }
            set { SetValue(CompassEnabledEnabledProperty, value); }
        }

        public static readonly BindableProperty RotateGesturesEnabledProperty = BindableProperty.Create(
            nameof(RotateGesturesEnabled), typeof(bool), typeof(UiSettings), true);
        public bool RotateGesturesEnabled
        {
            get { return (bool)GetValue(RotateGesturesEnabledProperty); }
            set { SetValue(RotateGesturesEnabledProperty, value); }
        }

        public static readonly BindableProperty MyLocationButtonEnabledProperty = BindableProperty.Create(
            nameof(MyLocationButtonEnabled), typeof(bool), typeof(UiSettings), false);
        public bool MyLocationButtonEnabled
        {
            get { return (bool)GetValue(MyLocationButtonEnabledProperty); }
            set { SetValue(MyLocationButtonEnabledProperty, value); }
        }

        public static readonly BindableProperty IndoorLevelPickerEnabledProperty = BindableProperty.Create(
            nameof(IndoorLevelPickerEnabled), typeof(bool), typeof(UiSettings), false);
        public bool IndoorLevelPickerEnabled
        {
            get { return (bool)GetValue(IndoorLevelPickerEnabledProperty); }
            set { SetValue(IndoorLevelPickerEnabledProperty, value); }
        }

        public static readonly BindableProperty ScrollGesturesEnabledProperty = BindableProperty.Create(
            nameof(ScrollGesturesEnabled), typeof(bool), typeof(UiSettings), true);
        public bool ScrollGesturesEnabled
        {
            get { return (bool)GetValue(ScrollGesturesEnabledProperty); }
            set { SetValue(ScrollGesturesEnabledProperty, value); }
        }

        public static readonly BindableProperty TiltGesturesEnabledProperty = BindableProperty.Create(
            nameof(TiltGesturesEnabled), typeof(bool), typeof(UiSettings), false);
        public bool TiltGesturesEnabled
        {
            get { return (bool)GetValue(TiltGesturesEnabledProperty); }
            set { SetValue(TiltGesturesEnabledProperty, value); }
        }

        public static readonly BindableProperty ZoomControlsEnabledProperty = BindableProperty.Create(
            nameof(ZoomControlsEnabled), typeof(bool), typeof(UiSettings), true);
        public bool ZoomControlsEnabled
        {
            get { return (bool)GetValue(ZoomControlsEnabledProperty); }
            set { SetValue(ZoomControlsEnabledProperty, value); }
        }

        public static readonly BindableProperty ZoomGesturesEnabledProperty = BindableProperty.Create(
            nameof(ZoomGesturesEnabled), typeof(bool), typeof(UiSettings), true);
        public bool ZoomGesturesEnabled
        {
            get { return (bool)GetValue(ZoomGesturesEnabledProperty); }
            set { SetValue(ZoomGesturesEnabledProperty, value); }
        }

        public static readonly BindableProperty MapToolbarEnabledProperty = BindableProperty.Create(
            nameof(MapToolbarEnabled), typeof(bool), typeof(UiSettings), false);
        public bool MapToolbarEnabled
        {
            get { return (bool)GetValue(MapToolbarEnabledProperty); }
            set { SetValue(MapToolbarEnabledProperty, value); }
        }

        internal UiSettings()
        {
        }
    }
}
