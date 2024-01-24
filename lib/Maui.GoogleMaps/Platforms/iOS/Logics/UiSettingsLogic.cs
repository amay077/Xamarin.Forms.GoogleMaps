
namespace Maui.GoogleMaps.Logics.iOS;

internal sealed class UiSettingsLogic : BaseUiSettingsLogic<Google.Maps.MapView>
{
    // These properties are 'null' when call Initialize()
    // If 'not null' then set true/false in app's page constructor.
    public bool? RotateGesturesEnabled { get; private set; }
    public bool? MyLocationButtonEnabled { get; private set; }
    public bool? ScrollGesturesEnabled { get; private set; }
    public bool? ZoomGesturesEnabled { get; private set; }

    protected override void OnUpdateCompassEnabled()
    {
        NativeMap.Settings.CompassButton = Map.UiSettings.CompassEnabled;
    }

    protected override void OnUpdateRotateGesturesEnabled()
    {
        NativeMap.Settings.RotateGestures = Map.UiSettings.RotateGesturesEnabled;
        RotateGesturesEnabled = Map.UiSettings.RotateGesturesEnabled;
    }

    protected override void OnUpdateMyLocationButtonEnabled()
    {
        NativeMap.Settings.MyLocationButton = Map.UiSettings.MyLocationButtonEnabled;
        MyLocationButtonEnabled = Map.UiSettings.MyLocationButtonEnabled;
    }

    protected override void OnUpdateIndoorLevelPickerEnabled()
    {
        NativeMap.Settings.IndoorPicker = Map.UiSettings.IndoorLevelPickerEnabled;
    }

    protected override void OnUpdateScrollGesturesEnabled()
    {
        NativeMap.Settings.ScrollGestures = Map.UiSettings.ScrollGesturesEnabled;
        ScrollGesturesEnabled = Map.UiSettings.ScrollGesturesEnabled;
    }

    protected override void OnUpdateTiltGesturesEnabled()
    {
        NativeMap.Settings.TiltGestures = Map.UiSettings.TiltGesturesEnabled;
    }

    protected override void OnUpdateZoomControlsEnabled()
    {
    }

    protected override void OnUpdateZoomGesturesEnabled()
    {
        NativeMap.Settings.ZoomGestures = Map.UiSettings.ZoomGesturesEnabled;
        ZoomGesturesEnabled = Map.UiSettings.ZoomGesturesEnabled;
    }

    protected override void OnUpdateMapToolbarEnabled()
    {
        // no-op
    }
}
