using System;
using System.Collections.Generic;
using Android.Gms.Maps;
using Xamarin.Forms.GoogleMaps.Logics;
using NativeGroundOverlay = Android.Gms.Maps.Model.GroundOverlay;
using Android.Gms.Maps.Model;
using Xamarin.Forms.GoogleMaps.Android.Extensions;
using Xamarin.Forms.GoogleMaps.Android;
using System.Linq;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class GroundOverlayLogic : DefaultLogic<GroundOverlay, NativeGroundOverlay, GoogleMap>
    {
        protected override IList<GroundOverlay> GetItems(Map map) => map.GroundOverlays;

        internal override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.GroundOverlayClick += OnGroundOverlayClick;
            }
        }

        internal override void Unregister(GoogleMap nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.GroundOverlayClick -= OnGroundOverlayClick;
            }

            base.Unregister(nativeMap, map);
        }

        protected override NativeGroundOverlay CreateNativeItem(GroundOverlay outerItem)
        {
            var opts = new GroundOverlayOptions()
                .PositionFromBounds(outerItem.Bounds.ToLatLngBounds())
                .Clickable(outerItem.IsClickable)
                .InvokeBearing(outerItem.Bearing)
                .InvokeImage(outerItem.Icon.ToBitmapDescriptor())
                .InvokeTransparency(outerItem.Transparency);

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

        void OnGroundOverlayClick(object sender, GoogleMap.GroundOverlayClickEventArgs e)
        {
            // clicked ground overlay
            var nativeItem = e.GroundOverlay;

            // lookup overlay
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => ((NativeGroundOverlay)outerItem.NativeObject).Id == nativeItem.Id);

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetOuterItem?.SendTap();
        }

        protected override void OnItemPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);

            var overlay = sender as GroundOverlay;
            var nativeOverlay = overlay?.NativeObject as NativeGroundOverlay;

            if (nativeOverlay == null)
                return;

            if (e.PropertyName == GroundOverlay.BearingProperty.PropertyName)
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


