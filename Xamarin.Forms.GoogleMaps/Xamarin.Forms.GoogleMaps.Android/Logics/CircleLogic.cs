using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Gms.Maps.Model;
using Xamarin.Forms.Platform.Android;
using NativeCircle = Android.Gms.Maps.Model.Circle;
using Xamarin.Forms.GoogleMaps.Android;
using Android.Gms.Maps;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class CircleLogic : DefaultCircleLogic<NativeCircle, GoogleMap>
    {
        protected override IList<Circle> GetItems(Map map) => map.Circles;

        protected override NativeCircle CreateNativeItem(Circle outerItem)
        {
            var opts = new CircleOptions();

            opts.InvokeCenter(new LatLng(outerItem.Center.Latitude, outerItem.Center.Longitude));
            opts.InvokeRadius(outerItem.Radius.Meters);
            opts.InvokeStrokeWidth(outerItem.StrokeWidth * this.ScaledDensity); // TODO: convert from px to pt. Is this collect? (looks like same iOS Maps)
            opts.InvokeStrokeColor(outerItem.StrokeColor.ToAndroid());
            opts.InvokeFillColor(outerItem.FillColor.ToAndroid());
            //opts.Clickable(outerItem.IsClickable);

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
    }
}

