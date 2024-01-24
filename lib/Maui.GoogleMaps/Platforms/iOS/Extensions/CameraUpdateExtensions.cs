using Maui.GoogleMaps.Internals;
using GCameraUpdate = Google.Maps.CameraUpdate;

namespace Maui.GoogleMaps.iOS.Extensions;

internal static class CameraUpdateExtensions
{
    public static GCameraUpdate ToIOS(this CameraUpdate self)
    {
        return self.UpdateType switch
        {
            CameraUpdateType.LatLng => GCameraUpdate.SetTarget(self.Position.ToCoord()),
            CameraUpdateType.LatLngZoom => GCameraUpdate.SetTarget(self.Position.ToCoord(), (float)self.Zoom),
            CameraUpdateType.LatLngBounds => GCameraUpdate.FitBounds(self.Bounds.ToCoordinateBounds(), self.Padding),
            CameraUpdateType.CameraPosition => GCameraUpdate.SetCamera(self.CameraPosition.ToIOS()),
            _ => throw new ArgumentException($"{nameof(self)} UpdateType is not supported."),
        };
    }
}