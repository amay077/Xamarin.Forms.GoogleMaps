using System;
using System.Collections.Generic;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.Logics;
using NativeGroundOverlay = Google.Maps.GroundOverlay;
using Xamarin.Forms.GoogleMaps.iOS.Extensions;
using CoreGraphics;

namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
    internal class GroundOverlayLogic : DefaultLogic<GroundOverlay, NativeGroundOverlay, MapView>
    {
        protected override IList<GroundOverlay> GetItems(Map map) => map.GroundOverlays;

        protected override NativeGroundOverlay CreateNativeItem(GroundOverlay outerItem)
        {
            var nativeOverlay = NativeGroundOverlay.GetGroundOverlay(
                outerItem.Bounds.ToCoordinateBounds(), outerItem.Icon.ToUIImage());
            nativeOverlay.Anchor = new CGPoint(outerItem.Anchor.X, outerItem.Anchor.Y);
            nativeOverlay.Bearing = outerItem.Bearing;
            nativeOverlay.Opacity = outerItem.Transparency;
            nativeOverlay.Tappable = outerItem.IsClickable;

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
    }
}

