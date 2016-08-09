using System;
using System.Collections.Generic;
using Android.Gms.Maps;
using Xamarin.Forms.GoogleMaps.Logics;
using NativeGroundOverlay = Android.Gms.Maps.Model.GroundOverlay;
using Android.Gms.Maps.Model;
using Xamarin.Forms.GoogleMaps.Android.Extensions;
using Xamarin.Forms.GoogleMaps.Android;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class GroundOverlayLogic : DefaultLogic<GroundOverlay, NativeGroundOverlay, GoogleMap>
    {
        protected override IList<GroundOverlay> GetItems(Map map) => map.GroundOverlays;

        protected override NativeGroundOverlay CreateNativeItem(GroundOverlay outerItem)
        {
            var opts = new GroundOverlayOptions()
                .PositionFromBounds(outerItem.Bounds.ToLatLngBounds())
                .Clickable(outerItem.IsClickable)
                .InvokeBearing(outerItem.Bearing)
                .InvokeImage(outerItem.Icon.ToBitmapDescriptor())
                .InvokeTransparency(outerItem.Transparency)
                .Anchor((float)outerItem.Anchor.X, (float)outerItem.Anchor.Y);

            var overlay = NativeMap.AddGroundOverlay(opts);

            // associate pin with marker for later lookup in event handlers
            outerItem.NativeObject = overlay;
            return overlay;
        }

        protected override NativeGroundOverlay DeleteNativeItem(GroundOverlay outerItem)
        {
            var nativeOverlay = outerItem.NativeObject as NativeGroundOverlay;
            if (nativeOverlay == null)
                return null;
            nativeOverlay.Remove();
            outerItem.NativeObject = null;

            return nativeOverlay;
        }

        protected override void OnItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);

            var overlay = sender as GroundOverlay;
            var nativeOverlay = overlay?.NativeObject as NativeGroundOverlay;

            if (nativeOverlay == null)
                return;

            if (e.PropertyName == GroundOverlay.AnchorProperty.PropertyName)
            {
                /* noop */
            }
            else if (e.PropertyName == GroundOverlay.BearingProperty.PropertyName)
            {
                nativeOverlay.Bearing = overlay.Bearing;
            }
            else if (e.PropertyName == GroundOverlay.BoundsProperty.PropertyName)
            {
                nativeOverlay.SetPositionFromBounds(overlay.Bounds.ToLatLngBounds());
            }
            else if (e.PropertyName == GroundOverlay.IconProperty.PropertyName)
            {
                nativeOverlay.SetImage(overlay.Icon.ToBitmapDescriptor());
            }
            else if (e.PropertyName == GroundOverlay.IsClickableProperty.PropertyName)
            {
                nativeOverlay.Clickable = overlay.IsClickable;
            }
            else if (e.PropertyName == GroundOverlay.TransparencyProperty.PropertyName)
            {
                nativeOverlay.Transparency = overlay.Transparency;
            }
        }
    }
}


