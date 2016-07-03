using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Gms.Maps.Model;
using Xamarin.Forms.Platform.Android;
using NativeCircle = Android.Gms.Maps.Model.Circle;
using Xamarin.Forms.GoogleMaps.Android;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
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

        internal override void OnElementPropertyChanged(PropertyChangedEventArgs e)
        {
        }

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);
            var circle = sender as Circle;
            var nativeCircle = circle?.NativeObject as NativeCircle;

            if (nativeCircle == null)
                return;

            if (e.PropertyName == Circle.StrokeWidthProperty.PropertyName)
            {
                nativeCircle.StrokeWidth = circle.StrokeWidth;
            }
            else if (e.PropertyName == Circle.StrokeColorProperty.PropertyName)
            {
                nativeCircle.StrokeColor = circle.StrokeColor.ToAndroid();
            }
            else if (e.PropertyName == Circle.FillColorProperty.PropertyName)
            {
                nativeCircle.FillColor = circle.FillColor.ToAndroid();
            }
            else if (e.PropertyName == Circle.CenterProperty.PropertyName)
            {
                nativeCircle.Center = circle.Center.ToLatLng();
            }
            else if (e.PropertyName == Circle.RadiusProperty.PropertyName)
            {
                nativeCircle.Radius = circle.Radius.Meters;
            }
        }
    }
}

