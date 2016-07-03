using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Xamarin.Forms.Platform.Android;
using NativePolyline = Android.Gms.Maps.Model.Polyline;

namespace Xamarin.Forms.GoogleMaps.Android
{
    public class PolylineLogic : ShapeLogic<Polyline, NativePolyline>
    {
        protected override IList<Polyline> GetItems(Map map) 
        {
            return map.Polylines; 
        }

        public PolylineLogic()
        {
        }

        internal override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.PolylineClick += MapOnPolylineClick;
            }
        }

        internal override void Unregister(GoogleMap nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.PolylineClick -= MapOnPolylineClick;
            }

            base.Unregister(nativeMap, map);
        }

        protected override NativePolyline CreateNativeItem(Polyline outerShape)
        {
            var opts = new PolylineOptions();

            foreach (var p in outerShape.Positions)
                opts.Add(new LatLng(p.Latitude, p.Longitude));

            opts.InvokeWidth(outerShape.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps) 
            opts.InvokeColor(outerShape.StrokeColor.ToAndroid());
            opts.Clickable(outerShape.IsClickable);

            var nativePolyline = NativeMap.AddPolyline(opts);

            // associate pin with marker for later lookup in event handlers
            outerShape.NativeObject = nativePolyline;
            return nativePolyline;
        }

        protected override NativePolyline DeleteNativeItem(Polyline outerShape)
        {
            var nativeShape = outerShape.NativeObject as NativePolyline;
            if (nativeShape == null)
                return null;

            nativeShape.Remove();
            return nativeShape;
        }

        void MapOnPolylineClick(object sender, GoogleMap.PolylineClickEventArgs eventArgs)
        {
            // clicked polyline
            var nativeItem = eventArgs.Polyline;

            // lookup pin
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => ((NativePolyline)outerItem.NativeObject).Id == nativeItem.Id);

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetOuterItem?.SendTap();
        }
    }
}

