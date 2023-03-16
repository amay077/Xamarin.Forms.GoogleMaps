using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.iOS.Extensions;
using Xamarin.Forms.Platform.iOS;
using NativePolyline = Google.Maps.Polyline;

namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
    public class PolylineLogic : DefaultPolylineLogic<NativePolyline, MapView>
    {
        protected override IList<Polyline> GetItems(Map map) => map.Polylines;

        public override void Register(MapView oldNativeMap, Map oldMap, MapView newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.OverlayTapped += OnOverlayTapped;
            }
        }

        public override void Unregister(MapView nativeMap, Map map)
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
            nativePolyline.ZIndex = outerItem.ZIndex;
            nativePolyline.Geodesic = outerItem.IsGeodesic;

            outerItem.NativeObject = nativePolyline;
            nativePolyline.Map = NativeMap;

            outerItem.SetOnPositionsChanged((polyline, e) =>
            {
                var native = polyline.NativeObject as NativePolyline;
                native.Path = polyline.Positions.ToMutablePath();
            });

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

        protected override void OnUpdateIsClickable(Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.Tappable = outerItem.IsClickable;
        }

        protected override void OnUpdateStrokeColor(Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.StrokeColor = outerItem.StrokeColor.ToUIColor();
        }

        protected override void OnUpdateStrokeWidth(Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.StrokeWidth = outerItem.StrokeWidth;
        }

        protected override void OnUpdateZIndex(Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }

        protected override void OnUpdateIsGeodesic(Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.Geodesic = outerItem.IsGeodesic;
        }
    }
}

