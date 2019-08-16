using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Platform.UWP;

namespace Xamarin.Forms.GoogleMaps.Logics.UWP
{
    internal class CircleLogic : DefaultCircleLogic<MapPolygon, MapControl>
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

        protected override IList<Circle> GetItems(Map map) => map.Circles;

        Geopath GenerateCircleGeopath(Position position, double radius)
        {
            var positions = new List<BasicGeoposition>();
            double latitude = position.Latitude * Math.PI / 180.0;
            double longitude = position.Longitude * Math.PI / 180.0;
            double x = radius / 6371000.0;
            for (int i = 0; i <= 360; i++)
            {
                double aRads = i * Math.PI / 180.0;
                double latRadians = Math.Asin(Math.Sin(latitude) * Math.Cos(x) + Math.Cos(latitude) * Math.Sin(x) * Math.Cos(aRads));
                double lngRadians = longitude + Math.Atan2(Math.Sin(aRads) * Math.Sin(x) * Math.Cos(latitude), Math.Cos(x) - Math.Sin(latitude) * Math.Sin(latRadians));
                BasicGeoposition loc = new BasicGeoposition() { Latitude = 180.0 * latRadians / Math.PI, Longitude = 180.0 * lngRadians / Math.PI };
                positions.Add(loc);
            }
            return new Geopath(positions);
        }

        protected override MapPolygon CreateNativeItem(Circle outerItem)
        {
            Color color = outerItem.StrokeColor;
            Color fillcolor = outerItem.FillColor;

            Geopath geopath = GenerateCircleGeopath(outerItem.Center, outerItem.Radius.Meters);

            MapPolygon polygon = new MapPolygon
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
                Path = geopath
            };

            NativeMap.MapElements.Add(polygon);

            outerItem.NativeObject = polygon;

            return polygon;
        }

        protected override MapPolygon DeleteNativeItem(Circle outerItem)
        {
            var polyline = outerItem.NativeObject as MapPolygon;

            if (polyline == null)
            {
                return null;
            }

            NativeMap.MapElements.Remove(polyline);

            outerItem.NativeObject = null;

            return polyline;
        }

        protected override void OnUpdateIsClickable(Circle outerItem, MapPolygon nativeItem)
        {
        }

        protected override void OnUpdateStrokeColor(Circle outerItem, MapPolygon nativeItem)
        {
            var color = outerItem.StrokeColor;

            nativeItem.StrokeColor = Windows.UI.Color.FromArgb(
                (byte)(color.A * 255),
                (byte)(color.R * 255),
                (byte)(color.G * 255),
                (byte)(color.B * 255));
        }

        protected override void OnUpdateStrokeWidth(Circle outerItem, MapPolygon nativeItem)
        {
            nativeItem.StrokeThickness = outerItem.StrokeWidth;
        }

        protected override void OnUpdateFillColor(Circle outerItem, MapPolygon nativeItem)
        {
            var color = outerItem.FillColor;

            nativeItem.FillColor = Windows.UI.Color.FromArgb(
                (byte)(color.A * 255),
                (byte)(color.R * 255),
                (byte)(color.G * 255),
                (byte)(color.B * 255));
        }

        protected override void OnUpdateZIndex(Circle outerItem, MapPolygon nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }

        protected override void OnUpdateCenter(Circle outerItem, MapPolygon nativeItem)
        {
            nativeItem.Path = GenerateCircleGeopath(outerItem.Center, outerItem.Radius.Meters);
        }

        protected override void OnUpdateRadius(Circle outerItem, MapPolygon nativeItem)
        {
            nativeItem.Path = GenerateCircleGeopath(outerItem.Center, outerItem.Radius.Meters);
        }
    }
}