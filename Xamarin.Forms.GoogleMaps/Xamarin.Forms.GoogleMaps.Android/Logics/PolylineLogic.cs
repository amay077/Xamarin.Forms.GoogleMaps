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
    internal class PolylineLogic : DefaultLogic<Polyline, NativePolyline, GoogleMap>
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

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);
            var polyline = sender as Polyline;
            var nativePolyline = polyline?.NativeObject as NativePolyline;

            if (nativePolyline == null)
                return;

            if (e.PropertyName == Polyline.StrokeWidthProperty.PropertyName)
            {
                nativePolyline.Width = polyline.StrokeWidth;
            }
            else if (e.PropertyName == Polyline.StrokeColorProperty.PropertyName)
            {
                nativePolyline.Color = polyline.StrokeColor.ToAndroid();
            }
            else if (e.PropertyName == Polyline.IsClickableProperty.PropertyName)
            {
                nativePolyline.Clickable = polyline.IsClickable;
            }
            else if (e.PropertyName == Polyline.ZIndexProperty.PropertyName)
            {
                nativePolyline.ZIndex = polyline.ZIndex;
            }
        }
    }
}

