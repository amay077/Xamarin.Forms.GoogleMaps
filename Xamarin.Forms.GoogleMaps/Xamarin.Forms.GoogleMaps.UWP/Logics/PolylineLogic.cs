using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Platform.UWP;

namespace Xamarin.Forms.GoogleMaps.Logics.UWP
{
    internal class PolylineLogic : DefaultPolylineLogic<MapPolyline, MapControl>
    {
        internal override void Register(MapControl oldNativeMap, Map oldMap, MapControl newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.MapElementClick += NewNativeMapOnMapElementClick;
            }
        }

        internal override void Unregister(MapControl nativeMap, Map map)
        {
            base.Unregister(nativeMap, map);

            if (nativeMap != null)
            {
                nativeMap.MapElementClick -= NewNativeMapOnMapElementClick;
            }
        }

        private void NewNativeMapOnMapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            var nativeItem = args.MapElements.FirstOrDefault(e => e is MapPolyline) as MapPolyline;

            if (nativeItem != null)
            {
                var targetOuterItem = GetItems(Map)
                    .FirstOrDefault(outerItem => ((MapPolyline) outerItem.NativeObject) == nativeItem && outerItem.IsClickable);

                targetOuterItem?.SendTap();
            }
        }

        protected override IList<Polyline> GetItems(Map map) => map.Polylines;

        protected override MapPolyline CreateNativeItem(Polyline outerItem)
        {
            Color color = outerItem.StrokeColor;
            Geopath geopath = new Geopath(outerItem.Positions.Select(position => new BasicGeoposition { Latitude = position.Latitude, Longitude = position.Longitude }));
            
            MapPolyline polyline = new MapPolyline
            {
                StrokeColor = Windows.UI.Color.FromArgb(
                    (byte)(color.A * 255),
                    (byte)(color.R * 255),
                    (byte)(color.G * 255),
                    (byte)(color.B * 255)),
                StrokeThickness = outerItem.StrokeWidth,
                ZIndex = outerItem.ZIndex,
                Path = geopath
            };

            NativeMap.MapElements.Add(polyline);

            outerItem.NativeObject = polyline;

            return polyline;
        }

        protected override MapPolyline DeleteNativeItem(Polyline outerItem)
        {
            var polyline = outerItem.NativeObject as MapPolyline;

            if (polyline == null)
            {
                return null;
            }

            NativeMap.MapElements.Remove(polyline);

            outerItem.NativeObject = null;

            return polyline;
        }

        internal override void OnUpdateIsClickable(Polyline outerItem, MapPolyline nativeItem)
        {
        }

        internal override void OnUpdateStrokeColor(Polyline outerItem, MapPolyline nativeItem)
        {
            var color = outerItem.StrokeColor;

            nativeItem.StrokeColor = Windows.UI.Color.FromArgb(
                (byte)(color.A * 255),
                (byte)(color.R * 255),
                (byte)(color.G * 255),
                (byte)(color.B * 255));
        }

        internal override void OnUpdateStrokeWidth(Polyline outerItem, MapPolyline nativeItem)
        {
            nativeItem.StrokeThickness = outerItem.StrokeWidth;
        }

        internal override void OnUpdateZIndex(Polyline outerItem, MapPolyline nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }
    }
}