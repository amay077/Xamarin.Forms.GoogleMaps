using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Xamarin.Forms.Platform.Android;
using NativePolygon = Android.Gms.Maps.Model.Polygon;

namespace Xamarin.Forms.GoogleMaps.Android.Logics
{
    internal class PolygonLogic : ShapeLogic<Polygon, NativePolygon>
    {
        public PolygonLogic()
        {
        }

        protected override IList<Polygon> GetItems(Map map) => map.Polygons;

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

        protected override NativePolygon CreateNativeItem(Polygon outerItem)
        {
            var opts = new PolygonOptions();

            foreach (var p in outerItem.Positions)
                opts.Add(new LatLng(p.Latitude, p.Longitude));

            opts.InvokeStrokeWidth(outerItem.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps) 
            opts.InvokeStrokeColor(outerItem.StrokeColor.ToAndroid());
            opts.InvokeFillColor(outerItem.FillColor.ToAndroid());
            opts.Clickable(outerItem.IsClickable);

            var nativePolygon = NativeMap.AddPolygon(opts);

            // associate pin with marker for later lookup in event handlers
            outerItem.NativeObject = nativePolygon;
            return nativePolygon;
        }

        protected override NativePolygon DeleteNativeItem(Polygon outerItem)
        {
            var nativePolygon = outerItem.NativeObject as NativePolygon;
            if (nativePolygon == null)
                return null;
            
            nativePolygon.Remove();
            outerItem.NativeObject = null;
            return nativePolygon;
        }

        void MapOnPolygonClick(object sender, GoogleMap.PolygonClickEventArgs eventArgs)
        {
            // clicked polyline
            var nativeItem = eventArgs.Polygon;

            // lookup pin
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => ((NativePolygon)outerItem.NativeObject).Id == nativeItem.Id);

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetOuterItem?.SendTap();
        }

        internal override void OnElementPropertyChanged(PropertyChangedEventArgs e)
        {
        }
    }
}

