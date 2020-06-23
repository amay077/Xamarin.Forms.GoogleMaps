using System;
using Google.Maps;
using GCameraPosition = Google.Maps.CameraPosition;

namespace Xamarin.Forms.GoogleMaps.iOS.Extensions
{
    internal static class CameraPositionExtensions
    {
        public static CameraPosition ToXamarinForms(this GCameraPosition self)
        {
            return new CameraPosition(
                    self.Target.ToPosition(),
                    self.Zoom,
                    self.Bearing,
                    self.ViewingAngle
            );
        }

        public static GCameraPosition ToIOS(this CameraPosition self)
        {
            return new GCameraPosition(self.Target.ToCoord(), (float)self.Zoom, (float)self.Bearing, (float)self.Tilt);
        }
    }
}
