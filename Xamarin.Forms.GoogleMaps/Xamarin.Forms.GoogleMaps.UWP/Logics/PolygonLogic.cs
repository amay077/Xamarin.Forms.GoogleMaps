using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Platform.UWP;

namespace Xamarin.Forms.GoogleMaps.Logics.UWP
{
    internal class PolygonLogic : DefaultPolygonLogic<MapPolygon, MapControl>
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
            var nativeItem = args.MapElements.FirstOrDefault(e => e is MapPolygon) as MapPolygon;

            if (nativeItem != null)
            {
                var targetOuterItem = GetItems(Map)
                    .FirstOrDefault(outerItem => ((MapPolygon) outerItem.NativeObject) == nativeItem && outerItem.IsClickable);
                
                targetOuterItem?.SendTap();
            }
        }

        protected override IList<Polygon> GetItems(Map map) => map.Polygons;

        protected override MapPolygon CreateNativeItem(Polygon outerItem)
        {
            Color color = outerItem.StrokeColor;
            Color fillcolor = outerItem.FillColor;
            Geopath geopath = new Geopath(outerItem.Positions.Select(position => new BasicGeoposition { Latitude = position.Latitude, Longitude = position.Longitude }));

            MapPolygon nativePolygon = new MapPolygon
            {
                FillColor = Windows.UI.Color.FromArgb(
                    (byte)(fillcolor.A * 255),
                    (byte)(fillcolor.R * 255),
                    (byte)(fillcolor.G * 255),
                    (byte)(fillcolor.B * 255)),
                StrokeColor = Windows.UI.Color.FromArgb(
                    (byte)(color.A * 255),
                    (byte)(color.R * 255),
                    (byte)(color.G * 255),
                    (byte)(color.B * 255)),
                StrokeThickness = outerItem.StrokeWidth,
                ZIndex = outerItem.ZIndex,
            };

            nativePolygon.Paths.Add(geopath);
            foreach (var hole in outerItem.Holes)
            {
                nativePolygon.Paths.Add(new Geopath(hole.Select(position => new BasicGeoposition { Latitude = position.Latitude, Longitude = position.Longitude })));
            }

            NativeMap.MapElements.Add(nativePolygon);
            
            outerItem.NativeObject = nativePolygon;

            return nativePolygon;
        }

        protected override MapPolygon DeleteNativeItem(Polygon outerItem)
        {
            var polygon = outerItem.NativeObject as MapPolygon;

            if (polygon == null)
            {
                return null;
            }

            NativeMap.MapElements.Remove(polygon);

            outerItem.NativeObject = null;

            return polygon;
        }

        internal override void OnUpdateIsClickable(Polygon outerItem, MapPolygon nativeItem)
        {
        }

        internal override void OnUpdateStrokeColor(Polygon outerItem, MapPolygon nativeItem)
        {
            var color = outerItem.StrokeColor;

            nativeItem.StrokeColor = Windows.UI.Color.FromArgb(
                (byte)(color.A * 255),
                (byte)(color.R * 255),
                (byte)(color.G * 255),
                (byte)(color.B * 255));
        }

        internal override void OnUpdateStrokeWidth(Polygon outerItem, MapPolygon nativeItem)
        {
            nativeItem.StrokeThickness = outerItem.StrokeWidth;
        }

        internal override void OnUpdateFillColor(Polygon outerItem, MapPolygon nativeItem)
        {
            var color = outerItem.FillColor;

            nativeItem.FillColor = Windows.UI.Color.FromArgb(
                (byte)(color.A * 255),
                (byte)(color.R * 255),
                (byte)(color.G * 255),
                (byte)(color.B * 255));
        }

        internal override void OnUpdateZIndex(Polygon outerItem, MapPolygon nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }

    }
}