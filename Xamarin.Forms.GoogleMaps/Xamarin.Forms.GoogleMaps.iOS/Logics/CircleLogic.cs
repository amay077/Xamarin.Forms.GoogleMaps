using System;
using System.Collections.Generic;
using System.ComponentModel;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.iOS.Extensions;
using NativeCircle = Google.Maps.Circle;
using System.Linq;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
    internal class CircleLogic : DefaultCircleLogic<NativeCircle, MapView>
    {
        protected override IList<Circle> GetItems(Map map) => map.Circles;

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

        protected override NativeCircle CreateNativeItem(Circle outerItem)
        {
            var nativeCircle = NativeCircle.FromPosition(
                outerItem.Center.ToCoord(), outerItem.Radius.Meters);
            nativeCircle.StrokeWidth = outerItem.StrokeWidth;
            nativeCircle.StrokeColor = outerItem.StrokeColor.ToUIColor();
            nativeCircle.FillColor = outerItem.FillColor.ToUIColor();
            nativeCircle.Tappable = outerItem.IsClickable;

            outerItem.NativeObject = nativeCircle;
            nativeCircle.Map = NativeMap;
            return nativeCircle;
        }

        protected override NativeCircle DeleteNativeItem(Circle outerItem)
        {
            var nativeCircle = outerItem.NativeObject as NativeCircle;
            nativeCircle.Map = null;
            return nativeCircle;
        }

        void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
        {
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
            targetOuterItem?.SendTap();
        }

        protected override void OnUpdateStrokeWidth(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.StrokeWidth = outerItem.StrokeWidth;

        protected override void OnUpdateStrokeColor(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.StrokeColor = outerItem.StrokeColor.ToUIColor();

        protected override void OnUpdateFillColor(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.FillColor = outerItem.FillColor.ToUIColor();

        protected override void OnUpdateCenter(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.Position = outerItem.Center.ToCoord();

        protected override void OnUpdateRadius(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.Radius = outerItem.Radius.Meters;

        protected override void OnUpdateIsClickable(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.Tappable = outerItem.IsClickable;

        protected override void OnUpdateZIndex(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.ZIndex = outerItem.ZIndex;
    }
}

