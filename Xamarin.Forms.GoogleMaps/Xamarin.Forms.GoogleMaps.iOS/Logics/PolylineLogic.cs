﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Google.Maps;
using Xamarin.Forms.Platform.iOS;
using NativePolyline = Google.Maps.Polyline;

namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
    internal class PolylineLogic : DefaultLogic<Polyline, NativePolyline, MapView>
    {
        protected override IList<Polyline> GetItems(Map map) => map.Polylines;

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

        protected override NativePolyline CreateNativeItem(Polyline outerItem)
        {
            var path = new MutablePath();
            foreach (var p in outerItem.Positions)
                path.AddLatLon(p.Latitude, p.Longitude);

            var nativePolyline = NativePolyline.FromPath(path);
            nativePolyline.StrokeWidth = outerItem.StrokeWidth;
            nativePolyline.StrokeColor = outerItem.StrokeColor.ToUIColor();
            nativePolyline.Tappable = outerItem.IsClickable;

            outerItem.NativeObject = nativePolyline;
            nativePolyline.Map = NativeMap;

            return nativePolyline;
        }

        protected override NativePolyline DeleteNativeItem(Polyline outerItem)
        {
            var nativePolyline = outerItem.NativeObject as NativePolyline;
            nativePolyline.Map = null;
            return nativePolyline;
        }

        void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
        {
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
            targetOuterItem?.SendTap();
        }
    }
}

