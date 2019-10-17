﻿using System;
using GCameraUpdateFactory = Android.Gms.Maps.CameraUpdateFactory;
using GCameraUpdate = Android.Gms.Maps.CameraUpdate;
using Xamarin.Forms.GoogleMaps.Internals;

namespace Xamarin.Forms.GoogleMaps.Android.Extensions
{
    internal static class CameraUpdateExtensions
    {
        public static GCameraUpdate ToAndroid(this CameraUpdate self, float scaledDensity)
        {
            try
            {
                switch (self.UpdateType)
                {
                    case CameraUpdateType.LatLng:
                        return GCameraUpdateFactory.NewLatLng(self.Position.ToLatLng());
                    case CameraUpdateType.LatLngZoom:
                        return GCameraUpdateFactory.NewLatLngZoom(self.Position.ToLatLng(), (float)self.Zoom);
                    case CameraUpdateType.LatLngBounds:
                        return GCameraUpdateFactory.NewLatLngBounds(self.Bounds.ToLatLngBounds(), (int)(self.Padding * scaledDensity)); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps)
                    case CameraUpdateType.CameraPosition:
                        return GCameraUpdateFactory.NewCameraPosition(self.CameraPosition.ToAndroid());
                    default:
                        throw new ArgumentException($"{nameof(self)} UpdateType is not supported.");
                }
            }
            catch(Java.Lang.IllegalStateException ex)
            {
                Console.WriteLine("Error using newLatLngBounds(LatLngBounds, int): Map size can't be 0. ");
                return null;
            }
        }
    }
}
