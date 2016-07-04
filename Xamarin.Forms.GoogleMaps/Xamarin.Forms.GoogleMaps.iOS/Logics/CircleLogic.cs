using System;
using System.Collections.Generic;
using System.ComponentModel;
using CoreLocation;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.Extensions.iOS;
using NativeCircle = Google.Maps.Circle;
using System.Linq;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
    internal class CircleLogic : DefaultLogic<Circle, NativeCircle>
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
            //nativeCircle.Tappable = outerItem.IsClickable;

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

        internal override void OnElementPropertyChanged(PropertyChangedEventArgs e)
        {
        }
    }
}

