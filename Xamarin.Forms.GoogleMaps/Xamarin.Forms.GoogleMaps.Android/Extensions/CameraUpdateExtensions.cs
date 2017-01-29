using System;
using GCameraUpdateFactory = Android.Gms.Maps.CameraUpdateFactory;
using GCameraUpdate = Android.Gms.Maps.CameraUpdate;
using Xamarin.Forms.GoogleMaps.Internals;

namespace Xamarin.Forms.GoogleMaps.Android.Extensions
{
    internal static class CameraUpdateExtensions
    {
        public static GCameraUpdate ToAndroid(this CameraUpdate self)
        {
            switch (self.UpdateType)
            {
                case CameraUpdateType.LatLng:
                    return GCameraUpdateFactory.NewLatLng(self.Position.ToLatLng());
                default:
                    throw new ArgumentException($"{nameof(self)} UpdateType is not supported.");
            }
        }
    }
}
