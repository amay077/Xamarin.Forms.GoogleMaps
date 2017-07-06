using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Gms.Maps.Model;
using Xamarin.Forms.Platform.Android;
using NativeCircle = Android.Gms.Maps.Model.Circle;
using Xamarin.Forms.GoogleMaps.Android;
using Android.Gms.Maps;
using System.Linq;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class CircleLogic : DefaultCircleLogic<NativeCircle, GoogleMap>
    {
        internal override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.CircleClick += OnCircleClick;
            }
        }

        internal override void Unregister(GoogleMap nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.CircleClick -= OnCircleClick;
            }

            base.Unregister(nativeMap, map);
        }

        protected override IList<Circle> GetItems(Map map) => map.Circles;

        protected override NativeCircle CreateNativeItem(Circle outerItem)
        {
            var opts = new CircleOptions();

            opts.InvokeCenter(new LatLng(outerItem.Center.Latitude, outerItem.Center.Longitude));
            opts.InvokeRadius(outerItem.Radius.Meters);
            opts.InvokeStrokeWidth(outerItem.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps)
            opts.InvokeStrokeColor(outerItem.StrokeColor.ToAndroid());
            opts.InvokeFillColor(outerItem.FillColor.ToAndroid());
            opts.Clickable(outerItem.IsClickable);

            var nativeCircle = NativeMap.AddCircle(opts);

            // associate pin with marker for later lookup in event handlers
            outerItem.NativeObject = nativeCircle;
            return nativeCircle;
        }

        protected override NativeCircle DeleteNativeItem(Circle outerItem)
        {
            var nativeCircle = outerItem.NativeObject as NativeCircle;
            if (nativeCircle == null)
                return null;
            nativeCircle.Remove();
            return nativeCircle;
        }

        private void OnCircleClick(object sender, GoogleMap.CircleClickEventArgs e)
        {
            // clicked circle
            var nativeItem = e.Circle;

            // lookup circle
            var targetOuterItem = GetItems(Map).FirstOrDefault(
                outerItem => ((NativeCircle)outerItem.NativeObject).Id == nativeItem.Id);

            // only consider event handled if a handler is present.
            // Else allow default behavior of displaying an info window.
            targetOuterItem?.SendTap();
        }

        protected override void OnUpdateStrokeWidth(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.StrokeWidth = outerItem.StrokeWidth;

        protected override void OnUpdateStrokeColor(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.StrokeColor = outerItem.StrokeColor.ToAndroid();

        protected override void OnUpdateFillColor(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.FillColor = outerItem.FillColor.ToAndroid();

        protected override void OnUpdateCenter(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.Center = outerItem.Center.ToLatLng();

        protected override void OnUpdateRadius(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.Radius = outerItem.Radius.Meters;

        protected override void OnUpdateIsClickable(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.Clickable = outerItem.IsClickable;

        protected override void OnUpdateZIndex(Circle outerItem, NativeCircle nativeItem)
            => nativeItem.ZIndex = outerItem.ZIndex;
    }
}

