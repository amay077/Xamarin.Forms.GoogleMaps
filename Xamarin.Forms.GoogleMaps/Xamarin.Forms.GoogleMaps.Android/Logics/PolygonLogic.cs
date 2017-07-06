using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Runtime;
using Xamarin.Forms.GoogleMaps.Android;
using Xamarin.Forms.GoogleMaps.Android.Extensions;
using Xamarin.Forms.Platform.Android;
using NativePolygon = Android.Gms.Maps.Model.Polygon;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class PolygonLogic : DefaultPolygonLogic<NativePolygon, GoogleMap>
    {
        protected override IList<Polygon> GetItems(Map map) => map.Polygons;

        internal override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.PolygonClick += OnPolygonClick;
            }
        }

        internal override void Unregister(GoogleMap nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.PolygonClick -= OnPolygonClick;
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

            foreach (var hole in outerItem.Holes)
            {
                opts.Holes.Add(hole.Select(x => x.ToLatLng()).ToJavaList());
            }

            var nativePolygon = NativeMap.AddPolygon(opts);

            // associate pin with marker for later lookup in event handlers
            outerItem.NativeObject = nativePolygon;
            outerItem.SetOnPositionsChanged((polygon, e) =>
            {
                var native = polygon.NativeObject as NativePolygon;
                native.Points = polygon.Positions.ToLatLngs();
            });

            // FIXME workarround for #280
            //outerItem.SetOnHolesChanged((polygon, e) =>
            //{
            //    var native = polygon.NativeObject as NativePolygon;
            //    native.Holes = (IList<IList<LatLng>>)polygon.Holes
            //        .Select(x => (IList<LatLng>)x.Select(y=>y.ToLatLng()).ToJavaList())
            //        .ToJavaList();
            //});

            return nativePolygon;
        }

        protected override NativePolygon DeleteNativeItem(Polygon outerItem)
        {
            outerItem.SetOnHolesChanged(null);
            outerItem.SetOnPositionsChanged(null);

            var nativePolygon = outerItem.NativeObject as NativePolygon;
            if (nativePolygon == null)
                return null;

            nativePolygon.Remove();
            outerItem.NativeObject = null;
            return nativePolygon;
        }

        void OnPolygonClick(object sender, GoogleMap.PolygonClickEventArgs e)
        {
            // clicked polyline
            var nativeItem = e.Polygon;

            // lookup pin
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => ((NativePolygon)outerItem.NativeObject).Id == nativeItem.Id);

            // only consider event handled if a handler is present.
            // Else allow default behavior of displaying an info window.
            targetOuterItem?.SendTap();
        }

        internal override void OnUpdateIsClickable(Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.Clickable = outerItem.IsClickable;
        }

        internal override void OnUpdateStrokeColor(Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.StrokeColor = outerItem.StrokeColor.ToAndroid();
        }

        internal override void OnUpdateStrokeWidth(Polygon outerItem, NativePolygon nativeItem)
        {
            // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps)
            nativeItem.StrokeWidth = outerItem.StrokeWidth * this.ScaledDensity;
        }

        internal override void OnUpdateFillColor(Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.FillColor = outerItem.FillColor.ToAndroid();
        }

        internal override void OnUpdateZIndex(Polygon outerItem, NativePolygon nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }
    }
}

