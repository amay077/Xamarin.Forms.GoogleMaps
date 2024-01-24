using GCameraPosition = Android.Gms.Maps.Model.CameraPosition;

namespace Maui.GoogleMaps.Android.Extensions;

internal static class CameraPositionExtensions
{
    public static CameraPosition ToMaui(this GCameraPosition self)
    {
        return new CameraPosition(
            self.Target.ToPosition(),
            self.Zoom,
            self.Bearing,
            self.Tilt
        );
    }

    public static GCameraPosition ToAndroid(this CameraPosition self)
    {
        return new GCameraPosition(self.Target.ToLatLng(), (float)self.Zoom, (float)self.Tilt, (float)self.Bearing);
    }
}