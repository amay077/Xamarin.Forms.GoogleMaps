using Android.Gms.Maps;

namespace Maui.GoogleMaps.Logics.Android;

internal sealed class UiSettingsLogic : BaseUiSettingsLogic<GoogleMap>
{
    // These properties are 'null' when call Initialize()
    // If 'not null' then set true/false in app's page constructor.
    public bool? RotateGesturesEnabled { get; private set; }
    public bool? MyLocationButtonEnabled { get; private set; }
    public bool? ScrollGesturesEnabled { get; private set; }
    public bool? ZoomControlsEnabled { get; private set; }
    public bool? ZoomGesturesEnabled { get; private set; }

    public override void Initialize()
    {
        base.Initialize();
    }

    protected override void OnUpdateCompassEnabled()
    {
        NativeMap.UiSettings.CompassEnabled = Map.UiSettings.CompassEnabled;
    }

    protected override void OnUpdateRotateGesturesEnabled()
    {
        NativeMap.UiSettings.RotateGesturesEnabled = Map.UiSettings.RotateGesturesEnabled;
        RotateGesturesEnabled = Map.UiSettings.RotateGesturesEnabled;
    }

    protected override void OnUpdateMyLocationButtonEnabled()
    {
        NativeMap.UiSettings.MyLocationButtonEnabled = Map.UiSettings.MyLocationButtonEnabled;
        MyLocationButtonEnabled = Map.UiSettings.MyLocationButtonEnabled;
    }

    protected override void OnUpdateIndoorLevelPickerEnabled()
    {
        NativeMap.UiSettings.IndoorLevelPickerEnabled = Map.UiSettings.IndoorLevelPickerEnabled;
    }

    protected override void OnUpdateScrollGesturesEnabled()
    {
        NativeMap.UiSettings.ScrollGesturesEnabled = Map.UiSettings.ScrollGesturesEnabled;
        ScrollGesturesEnabled = Map.UiSettings.ScrollGesturesEnabled;
    }

    protected override void OnUpdateTiltGesturesEnabled()
    {
        NativeMap.UiSettings.TiltGesturesEnabled = Map.UiSettings.TiltGesturesEnabled;
    }

    protected override void OnUpdateZoomControlsEnabled()
    {
        NativeMap.UiSettings.ZoomControlsEnabled = Map.UiSettings.ZoomControlsEnabled;
        ZoomControlsEnabled = Map.UiSettings.ZoomControlsEnabled;
    }

    protected override void OnUpdateZoomGesturesEnabled()
    {
        NativeMap.UiSettings.ZoomGesturesEnabled = Map.UiSettings.ZoomGesturesEnabled;
        ZoomGesturesEnabled = Map.UiSettings.ZoomGesturesEnabled;
    }

    protected override void OnUpdateMapToolbarEnabled()
    {
        NativeMap.UiSettings.MapToolbarEnabled = Map.UiSettings.MapToolbarEnabled;
    }
}