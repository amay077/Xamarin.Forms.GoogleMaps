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
    public class PolylineLogic : ShapeLogic<Polyline>
    {
        List<NativePolyline> _polylines;

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

        protected override void AddItems(IList items)
        {
            var map = NativeMap;
            if (map == null)
                return;

            if (_polylines == null)
                _polylines = new List<NativePolyline>();

            _polylines.AddRange(items.Cast<Polyline>().Select(polyline =>
            {
                var opts = new PolylineOptions();

                foreach (var p in polyline.Positions)
                    opts.Add(new LatLng(p.Latitude, p.Longitude));

                opts.InvokeWidth(polyline.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps) 
                opts.InvokeColor(polyline.StrokeColor.ToAndroid());
                opts.Clickable(polyline.IsClickable);

                var nativePolyline = map.AddPolyline(opts);

                // associate pin with marker for later lookup in event handlers
                polyline.NativeObject = nativePolyline;
                return nativePolyline;
            }));
        }

        protected override void RemoveItems(IList items)
        {
            var map = NativeMap;
            if (map == null)
                return;
            if (_polylines == null)
                return;

            foreach (Polyline polyline in items)
            {
                var nativePolyline = _polylines.FirstOrDefault(m => ((NativePolyline)polyline.NativeObject).Id == m.Id);
                if (nativePolyline == null)
                    continue;
                nativePolyline.Remove();
                _polylines.Remove(nativePolyline);
            }
        }

        protected override void ResetItems()
        {
            _polylines?.ForEach(line => line.Remove());
            _polylines = null;
            AddItems((IList)GetItems(Map));
        }

        void MapOnPolylineClick(object sender, GoogleMap.PolylineClickEventArgs eventArgs)
        {
            // clicked polyline
            var clickedPolyline = eventArgs.Polyline;

            // lookup pin
            Polyline targetPolyline = null;
            for (var i = 0; i < GetItems(Map).Count; i++)
            {
                var line = GetItems(Map)[i];
                if (((NativePolyline)line.NativeObject).Id != clickedPolyline.Id)
                    continue;

                targetPolyline = line;
                break;
            }

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetPolyline?.SendTap();
        }
   }
}

