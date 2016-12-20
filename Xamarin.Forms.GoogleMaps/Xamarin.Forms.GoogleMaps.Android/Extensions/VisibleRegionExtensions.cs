using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps.Model;

namespace Xamarin.Forms.GoogleMaps.Android.Extensions
{
    internal static class VisibleRegionExtensions
    {
        public static Bounds ToBounds(this VisibleRegion self)
            => new Bounds(self.NearLeft.ToPosition(), self.FarRight.ToPosition());
        public static Bounds ToBounds(this LatLngBounds self)
            => new Bounds(self.Southwest.ToPosition(), self.Northeast.ToPosition());
    }
}