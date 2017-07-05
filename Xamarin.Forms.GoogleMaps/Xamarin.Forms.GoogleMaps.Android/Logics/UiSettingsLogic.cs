using System;
using Android.Gms.Maps;
using Xamarin.Forms.GoogleMaps.Logics;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal sealed class UiSettingsLogic : BaseUiSettingsLogic<GoogleMap>
    {
        protected override void OnUpdateCompassEnabled()
        {
            NativeMap.UiSettings.CompassEnabled = Map.UiSettings.CompassEnabled;
        }

        protected override void OnUpdateRotateGesturesEnabled()
        {
            NativeMap.UiSettings.RotateGesturesEnabled = Map.UiSettings.RotateGesturesEnabled;
        }

        protected override void OnUpdateMyLocationButtonEnabled()
        {
            NativeMap.UiSettings.MyLocationButtonEnabled = Map.UiSettings.MyLocationButtonEnabled;
        }

        protected override void OnUpdateIndoorLevelPickerEnabled()
        {
            NativeMap.UiSettings.IndoorLevelPickerEnabled = Map.UiSettings.IndoorLevelPickerEnabled;
        }

        protected override void OnUpdateScrollGesturesEnabled()
        {
            NativeMap.UiSettings.ScrollGesturesEnabled = Map.UiSettings.ScrollGesturesEnabled;
        }

        protected override void OnUpdateTiltGesturesEnabled()
        {
            NativeMap.UiSettings.TiltGesturesEnabled = Map.UiSettings.TiltGesturesEnabled;
        }

        protected override void OnUpdateZoomControlsEnabled()
        {
            NativeMap.UiSettings.ZoomControlsEnabled = Map.UiSettings.ZoomControlsEnabled;
        }

        protected override void OnUpdateZoomGesturesEnabled()
        {
            NativeMap.UiSettings.ZoomGesturesEnabled = Map.UiSettings.ZoomGesturesEnabled;
        }
    }
}
