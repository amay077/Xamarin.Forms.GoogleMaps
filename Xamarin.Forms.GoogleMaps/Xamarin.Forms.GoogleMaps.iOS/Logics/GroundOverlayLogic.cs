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
    internal class GroundOverlayLogic : DefaultLogic<GroundOverlay, NativeGroundOverlay, MapView>
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

        void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
        {
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
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
                nativeOverlay.Bounds = overlay.Bounds.ToCoordinateBounds();
            }
            else if (e.PropertyName == GroundOverlay.IconProperty.PropertyName)
            {
                nativeOverlay.Icon = overlay.Icon.ToUIImage();
            }
            else if (e.PropertyName == GroundOverlay.IsClickableProperty.PropertyName)
            {
                nativeOverlay.Tappable = overlay.IsClickable;
            }
            else if (e.PropertyName == GroundOverlay.TransparencyProperty.PropertyName)
            {
                nativeOverlay.Opacity = overlay.Transparency;
            }
        }
    }
}

