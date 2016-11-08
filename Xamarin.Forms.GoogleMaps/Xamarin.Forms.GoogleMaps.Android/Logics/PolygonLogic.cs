using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Xamarin.Forms.GoogleMaps.Android;
using Xamarin.Forms.Platform.Android;
using NativePolygon = Android.Gms.Maps.Model.Polygon;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class PolygonLogic : DefaultLogic<Polygon, NativePolygon, GoogleMap>
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

            var nativePolygon = NativeMap.AddPolygon(opts);

            // associate pin with marker for later lookup in event handlers
            outerItem.NativeObject = nativePolygon;
            outerItem.SetOnPositionsChanged((polygon, e) =>
            {
                var native = polygon.NativeObject as NativePolygon;
                native.Points = polygon.Positions.ToLatLngs();
            });

            return nativePolygon;
        }

        protected override NativePolygon DeleteNativeItem(Polygon outerItem)
        {
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

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);
            var polygon = sender as Polygon;
            var nativePolygon = polygon?.NativeObject as NativePolygon;

            if (nativePolygon == null)
                return;

            if (e.PropertyName == Polygon.StrokeWidthProperty.PropertyName)
            {
                nativePolygon.StrokeWidth = polygon.StrokeWidth;
            }
            else if (e.PropertyName == Polygon.StrokeColorProperty.PropertyName)
            {
                nativePolygon.StrokeColor = polygon.StrokeColor.ToAndroid();
            }
            else if (e.PropertyName == Polygon.FillColorProperty.PropertyName)
            {
                nativePolygon.FillColor = polygon.FillColor.ToAndroid();
            }
            else if (e.PropertyName == Polygon.IsClickableProperty.PropertyName)
            {
                nativePolygon.Clickable = polygon.IsClickable;
            }
        }
    }
}

