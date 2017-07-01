using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Xamarin.Forms.GoogleMaps.Android;
using Xamarin.Forms.Platform.Android;
using NativePolyline = Android.Gms.Maps.Model.Polyline;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class PolylineLogic : DefaultPolylineLogic<NativePolyline, GoogleMap>
    {
        protected override IList<Polyline> GetItems(Map map) => map.Polylines;

        internal override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.PolylineClick += OnPolylineClick;
            }
        }

        internal override void Unregister(GoogleMap nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.PolylineClick -= OnPolylineClick;
            }

            base.Unregister(nativeMap, map);
        }

        protected override NativePolyline CreateNativeItem(Polyline outerItem)
        {
            var opts = new PolylineOptions();

            foreach (var p in outerItem.Positions)
                opts.Add(new LatLng(p.Latitude, p.Longitude));

            opts.InvokeWidth(outerItem.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps)
            opts.InvokeColor(outerItem.StrokeColor.ToAndroid());
            opts.Clickable(outerItem.IsClickable);
            opts.InvokeZIndex(outerItem.ZIndex);

            var nativePolyline = NativeMap.AddPolyline(opts);

            // associate pin with marker for later lookup in event handlers
            outerItem.NativeObject = nativePolyline;
            outerItem.SetOnPositionsChanged((polyline, e) =>
            {
                var native = polyline.NativeObject as NativePolyline;
                native.Points = polyline.Positions.ToLatLngs();
            });

            return nativePolyline;
        }

        protected override NativePolyline DeleteNativeItem(Polyline outerItem)
        {
            outerItem.SetOnPositionsChanged(null);

            var nativeShape = outerItem.NativeObject as NativePolyline;
            if (nativeShape == null)
                return null;

            nativeShape.Remove();
            outerItem.NativeObject = null;
            return nativeShape;
        }

        void OnPolylineClick(object sender, GoogleMap.PolylineClickEventArgs e)
        {
            // clicked polyline
            var nativeItem = e.Polyline;

            // lookup pin
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => ((NativePolyline)outerItem.NativeObject).Id == nativeItem.Id);

            // only consider event handled if a handler is present.
            // Else allow default behavior of displaying an info window.
            targetOuterItem?.SendTap();
        }

        internal override void OnUpdateIsClickable(Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.Clickable = outerItem.IsClickable;
        }

        internal override void OnUpdateStrokeColor(Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.Color = outerItem.StrokeColor.ToAndroid();
        }

        internal override void OnUpdateStrokeWidth(Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.Width = outerItem.StrokeWidth;
        }

        internal override void OnUpdateZIndex(Polyline outerItem, NativePolyline nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }
    }
}

