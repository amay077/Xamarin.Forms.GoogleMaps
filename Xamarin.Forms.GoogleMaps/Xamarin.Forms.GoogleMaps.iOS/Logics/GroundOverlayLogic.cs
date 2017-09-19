using System;
using System.Collections.Generic;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.Logics;
using NativeGroundOverlay = Google.Maps.GroundOverlay;
using Xamarin.Forms.GoogleMaps.iOS.Extensions;
using CoreGraphics;
using System.Linq;

namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
    internal class GroundOverlayLogic : DefaultGroundOverlayLogic<NativeGroundOverlay, MapView>
    {
        protected override IList<GroundOverlay> GetItems(Map map) => map.GroundOverlays;

        internal override void Register(MapView oldNativeMap, Map oldMap, MapView newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.OverlayTapped += OnOverlayTapped;
            }
        }

        internal override void Unregister(MapView nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.OverlayTapped -= OnOverlayTapped;
            }

            base.Unregister(nativeMap, map);
        }

        protected override NativeGroundOverlay CreateNativeItem(GroundOverlay outerItem)
        {
            var nativeOverlay = NativeGroundOverlay.GetGroundOverlay(
                outerItem.Bounds.ToCoordinateBounds(), outerItem.Icon.ToUIImage());
            nativeOverlay.Bearing = outerItem.Bearing;
            nativeOverlay.Opacity = 1 - outerItem.Transparency;
            nativeOverlay.Tappable = outerItem.IsClickable;
            nativeOverlay.ZIndex = outerItem.ZIndex;

            outerItem.NativeObject = nativeOverlay;
            nativeOverlay.Map = NativeMap;
            return nativeOverlay;
        }

        protected override NativeGroundOverlay DeleteNativeItem(GroundOverlay outerItem)
        {
            var nativeOverlay = outerItem.NativeObject as NativeGroundOverlay;
            nativeOverlay.Map = null;

            return nativeOverlay;
        }

        void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
        {
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
            targetOuterItem?.SendTap();
        }

        internal override void OnUpdateBearing(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            nativeItem.Bearing = outerItem.Bearing;
        }

        internal override void OnUpdateBounds(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            nativeItem.Bounds = outerItem.Bounds.ToCoordinateBounds();
        }

        internal override void OnUpdateIcon(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            nativeItem.Icon = outerItem.Icon.ToUIImage();
        }

        internal override void OnUpdateIsClickable(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            nativeItem.Tappable = outerItem.IsClickable;
        }

        internal override void OnUpdateTransparency(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            nativeItem.Opacity = 1f - outerItem.Transparency;
        }

        internal override void OnUpdateZIndex(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }
    }
}

