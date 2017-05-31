using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;

namespace Xamarin.Forms.GoogleMaps.Logics.UWP
{
    internal sealed class UiSettingsLogic : BaseUiSettingsLogic<MapControl>
    {
        public UiSettingsLogic() : base()
        {
        }

        protected override void OnUpdateCompassEnabled()
        {
        }

        protected override void OnUpdateRotateGesturesEnabled()
        {
            NativeMap.RotateInteractionMode = Map.HasRotationEnabled ? 
                MapInteractionMode.Auto : MapInteractionMode.Disabled;

            if (Map.HasRotationEnabled != Map.UiSettings.RotateGesturesEnabled)
            {
                Map.HasRotationEnabled = Map.UiSettings.RotateGesturesEnabled;
            }
        }

        protected override void OnUpdateMyLocationButtonEnabled()
        {
        }

        protected override void OnUpdateIndoorLevelPickerEnabled()
        {
        }

        protected override void OnUpdateScrollGesturesEnabled()
        {
            NativeMap.PanInteractionMode = Map.UiSettings.ScrollGesturesEnabled ?
                MapPanInteractionMode.Auto : MapPanInteractionMode.Disabled;

            if (Map.HasScrollEnabled != Map.UiSettings.ScrollGesturesEnabled)
            {
                Map.HasScrollEnabled = Map.UiSettings.ScrollGesturesEnabled;
            }
        }

        protected override void OnUpdateTiltGesturesEnabled()
        {
            NativeMap.TiltInteractionMode = Map.UiSettings.TiltGesturesEnabled ?
                MapInteractionMode.GestureOnly : MapInteractionMode.Disabled;
        }

        protected override void OnUpdateZoomControlsEnabled()
        {
            UpdateZoomControlAndGesturesEnabled();
        }

        protected override void OnUpdateZoomGesturesEnabled()
        {
            UpdateZoomControlAndGesturesEnabled();
        }

        private void UpdateZoomControlAndGesturesEnabled()
        {
            if (Map.UiSettings.ZoomControlsEnabled && Map.UiSettings.ZoomGesturesEnabled)
            {
                NativeMap.ZoomInteractionMode = MapInteractionMode.GestureAndControl;
            }
            else if (Map.UiSettings.ZoomControlsEnabled && !Map.UiSettings.ZoomGesturesEnabled)
            {
                NativeMap.ZoomInteractionMode = MapInteractionMode.ControlOnly;
            }
            else if (!Map.UiSettings.ZoomControlsEnabled && Map.UiSettings.ZoomGesturesEnabled)
            {
                NativeMap.ZoomInteractionMode = MapInteractionMode.GestureOnly;
            }
            else
            {
                NativeMap.ZoomInteractionMode = MapInteractionMode.Disabled;
            }
        }
    }
}
