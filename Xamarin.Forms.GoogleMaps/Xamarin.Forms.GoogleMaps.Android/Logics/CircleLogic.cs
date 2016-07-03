using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Gms.Maps.Model;
using Xamarin.Forms.GoogleMaps.Android.Logics;
using Xamarin.Forms.Platform.Android;
using NativeCircle = Android.Gms.Maps.Model.Circle;

namespace Xamarin.Forms.GoogleMaps.Android
{
    internal class CircleLogic : ShapeLogic<Circle, NativeCircle>
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
            outerItem.Id = nativeCircle;
            return nativeCircle;
        }

        protected override NativeCircle DeleteNativeItem(Circle outerItem)
        {
            var nativeCircle = outerItem.Id as NativeCircle;
            if (nativeCircle == null)
                return null;
            nativeCircle.Remove();
            return nativeCircle;
        }

        internal override void OnElementPropertyChanged(PropertyChangedEventArgs e)
        {
        }
    }
}

