using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.iOS.Extensions;
using Xamarin.Forms.Platform.iOS;
using NativePolygon = Google.Maps.Polygon;

namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
    internal class PolygonLogic : DefaultLogic<Polygon, NativePolygon, MapView>
    {
        protected override IList<Polygon> GetItems(Map map) => map.Polygons;

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

        protected override NativePolygon CreateNativeItem(Polygon outerItem)
        {
            var nativePolygon = NativePolygon.FromPath(outerItem.Positions.ToMutablePath());
            nativePolygon.StrokeWidth = outerItem.StrokeWidth;
            nativePolygon.StrokeColor = outerItem.StrokeColor.ToUIColor();
            nativePolygon.FillColor = outerItem.FillColor.ToUIColor();
            nativePolygon.Tappable = outerItem.IsClickable;

            nativePolygon.Holes = outerItem.Holes
                .Select(hole => hole.ToMutablePath())
                .ToArray();

            outerItem.NativeObject = nativePolygon;
            nativePolygon.Map = NativeMap;

            outerItem.SetOnHolesChanged((polygon, e) =>
            {
                var native = polygon.NativeObject as NativePolygon;
                native.Holes = outerItem.Holes
                    .Select(hole => hole.ToMutablePath())
                    .ToArray();
            });

            return nativePolygon;
        }

        protected override NativePolygon DeleteNativeItem(Polygon outerItem)
        {
            outerItem.SetOnHolesChanged(null);

            var nativePolygon = outerItem.NativeObject as NativePolygon;
            nativePolygon.Map = null;
            return nativePolygon;
        }

        void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
        {
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
            targetOuterItem?.SendTap();
        }
    }
}

