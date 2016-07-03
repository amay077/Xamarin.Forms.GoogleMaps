using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Xamarin.Forms.Platform.Android;
using NativePolygon = Android.Gms.Maps.Model.Polygon;

namespace Xamarin.Forms.GoogleMaps.Android
{
    public class PolygonLogic : ShapeLogic<Polygon>
    {
        List<NativePolygon> _nativePolygons;

        protected override IList<Polygon> GetItems(Map map)
        {
            return map.Polygons;
        }

        public PolygonLogic()
        {
        }

        internal override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.PolygonClick += MapOnPolygonClick;
            }
        }

        internal override void Unregister(GoogleMap nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.PolygonClick -= MapOnPolygonClick;
            }

            base.Unregister(nativeMap, map);
        }

        protected override void AddItems(IList items)
        {
            var map = NativeMap;
            if (map == null)
                return;

            if (_nativePolygons == null)
                _nativePolygons = new List<NativePolygon>();

            _nativePolygons.AddRange(items.Cast<Polygon>().Select(polygon =>
            {
                var opts = new PolygonOptions();

                foreach (var p in polygon.Positions)
                    opts.Add(new LatLng(p.Latitude, p.Longitude));

                opts.InvokeStrokeWidth(polygon.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps) 
                opts.InvokeStrokeColor(polygon.StrokeColor.ToAndroid());
                opts.InvokeFillColor(polygon.FillColor.ToAndroid());
                opts.Clickable(polygon.IsClickable);

                var nativePolygon = map.AddPolygon(opts);

                // associate pin with marker for later lookup in event handlers
                polygon.Id = nativePolygon;
                return nativePolygon;
            }));
        }

        protected override void RemoveItems(IList items)
        {
            var map = NativeMap;
            if (map == null)
                return;
            if (_nativePolygons == null)
                return;

            foreach (Polygon polygon in items)
            {
                var nativePolygon = _nativePolygons.FirstOrDefault(m => ((NativePolygon)polygon.Id).Id == m.Id);
                if (nativePolygon == null)
                    continue;
                nativePolygon.Remove();
                _nativePolygons.Remove(nativePolygon);
            }
        }

        protected override void ResetItems()
        {
            _nativePolygons?.ForEach(polygon => polygon.Remove());
            _nativePolygons = null;
            AddItems((IList)Map.Polygons);
        }

        void MapOnPolygonClick(object sender, GoogleMap.PolygonClickEventArgs eventArgs)
        {
            // clicked Polygons
            var clickedPolygon = eventArgs.Polygon;

            // lookup pin
            Polygon targetPolygon = null;
            for (var i = 0; i < Map.Polygons.Count; i++)
            {
                var polygon = Map.Polygons[i];
                if (((NativePolygon)polygon.Id).Id != clickedPolygon.Id)
                    continue;

                targetPolygon = polygon;
                break;
            }

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetPolygon?.SendTap();
        }
    }
}

