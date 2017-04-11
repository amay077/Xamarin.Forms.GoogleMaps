using System;
using Xamarin.Forms.GoogleMaps.Internals;

using GCameraUpdate = Google.Maps.CameraUpdate;

namespace Xamarin.Forms.GoogleMaps.iOS.Extensions
{
    internal static class CameraUpdateExtensions
    {
        public static GCameraUpdate ToIOS(this CameraUpdate self)
        {
            switch (self.UpdateType)
            {
                case CameraUpdateType.LatLng:
                    return GCameraUpdate.SetTarget(self.Position.ToCoord());
                case CameraUpdateType.LatLngZoom:
                    return GCameraUpdate.SetTarget(self.Position.ToCoord(), (float)self.Zoom);
                case CameraUpdateType.LatLngBounds:
                    return GCameraUpdate.FitBounds(self.Bounds.ToCoordinateBounds(), self.Padding);
                case CameraUpdateType.CameraPosition:
                    return GCameraUpdate.SetCamera(self.CameraPosition.ToIOS());
                default:
                    throw new ArgumentException($"{nameof(self)} UpdateType is not supported.");
            }
        }

    }
}