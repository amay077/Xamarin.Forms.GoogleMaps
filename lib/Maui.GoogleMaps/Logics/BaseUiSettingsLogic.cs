using System.ComponentModel;

namespace Maui.GoogleMaps.Logics
{
    internal abstract class BaseUiSettingsLogic<TNativeMap>
    {
        protected Map Map { get; private set; }
        protected TNativeMap NativeMap { get; private set; }

        public void Register(Map map, TNativeMap nativeMap)
        {
            this.Map = map;
            this.NativeMap = nativeMap;

            map.UiSettings.PropertyChanged += UiSettings_PropertyChanged;
        }

        void UiSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == UiSettings.CompassEnabledEnabledProperty.PropertyName)
            {
                OnUpdateCompassEnabled();
            }
            else if (e.PropertyName == UiSettings.RotateGesturesEnabledProperty.PropertyName)
            {
                OnUpdateRotateGesturesEnabled();
            }
            else if (e.PropertyName == UiSettings.MyLocationButtonEnabledProperty.PropertyName)
            {
                OnUpdateMyLocationButtonEnabled();
            }
            else if (e.PropertyName == UiSettings.IndoorLevelPickerEnabledProperty.PropertyName)
            {
                OnUpdateIndoorLevelPickerEnabled();
            }
            else if (e.PropertyName == UiSettings.ScrollGesturesEnabledProperty.PropertyName)
            {
                OnUpdateScrollGesturesEnabled();
            }
            else if (e.PropertyName == UiSettings.TiltGesturesEnabledProperty.PropertyName)
            {
                OnUpdateTiltGesturesEnabled();
            }
            else if (e.PropertyName == UiSettings.ZoomControlsEnabledProperty.PropertyName)
            {
                OnUpdateZoomControlsEnabled();
            }
            else if (e.PropertyName == UiSettings.ZoomGesturesEnabledProperty.PropertyName)
            {
                OnUpdateZoomGesturesEnabled();
            }
            else if (e.PropertyName == UiSettings.MapToolbarEnabledProperty.PropertyName)
            {
                OnUpdateMapToolbarEnabled();
            }
        }

        public void Unregister()
        {
            if (this.Map == null)
            {
                return;
            }

            this.Map.UiSettings.PropertyChanged -= UiSettings_PropertyChanged;
        }

        public virtual void Initialize()
        {
            OnUpdateCompassEnabled();
            OnUpdateRotateGesturesEnabled();
            OnUpdateMyLocationButtonEnabled();
            OnUpdateIndoorLevelPickerEnabled();
            OnUpdateScrollGesturesEnabled();
            OnUpdateTiltGesturesEnabled();
            OnUpdateZoomControlsEnabled();
            OnUpdateZoomGesturesEnabled();
            OnUpdateMapToolbarEnabled();
        }

        abstract protected void OnUpdateCompassEnabled();
        abstract protected void OnUpdateRotateGesturesEnabled();
        abstract protected void OnUpdateMyLocationButtonEnabled();
        abstract protected void OnUpdateIndoorLevelPickerEnabled();
        abstract protected void OnUpdateScrollGesturesEnabled();
        abstract protected void OnUpdateTiltGesturesEnabled();
        abstract protected void OnUpdateZoomControlsEnabled();
        abstract protected void OnUpdateZoomGesturesEnabled();
        abstract protected void OnUpdateMapToolbarEnabled();
    }
}
