using System;
using GCameraPosition = Android.Gms.Maps.Model.CameraPosition;

namespace Xamarin.Forms.GoogleMaps.Android.Extensions
{
    internal static class CameraPositionExtensions
    {
        public static CameraPosition ToXamarinForms(this GCameraPosition self)
        {
            return new CameraPosition(
                self.Target.ToPosition(),
                self.Bearing,
                self.Tilt,
                self.Zoom
            );
        }
    }
}
