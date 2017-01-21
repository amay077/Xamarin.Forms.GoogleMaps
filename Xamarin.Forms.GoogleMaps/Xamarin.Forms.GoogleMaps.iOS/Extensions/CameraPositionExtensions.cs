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
                    self.Bearing,
                    self.ViewingAngle,
                    self.Zoom
            );
        }
    }
}
