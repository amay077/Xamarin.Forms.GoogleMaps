using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Xamarin.Forms.GoogleMaps.Logics
{
    internal abstract class DefaultPolylineLogic<TNative, TNativeMap> : DefaultLogic<Polyline, TNative, TNativeMap>
        where TNative : class
        where TNativeMap : class
    {
        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);
            var outerItem = sender as Polyline;
            var nativeItem = outerItem?.NativeObject as TNative;

            if (nativeItem == null)
                return;

            if (e.PropertyName == Polyline.IsClickableProperty.PropertyName) OnUpdateIsClickable(outerItem, nativeItem);
            else if (e.PropertyName == Polyline.StrokeColorProperty.PropertyName) OnUpdateStrokeColor(outerItem, nativeItem);
            else if (e.PropertyName == Polyline.StrokeWidthProperty.PropertyName) OnUpdateStrokeWidth(outerItem, nativeItem);
            else if (e.PropertyName == Polyline.ZIndexProperty.PropertyName) OnUpdateZIndex(outerItem, nativeItem);
        }

        internal abstract void OnUpdateIsClickable(Polyline outerItem, TNative nativeItem);
        internal abstract void OnUpdateStrokeColor(Polyline outerItem, TNative nativeItem);
        internal abstract void OnUpdateStrokeWidth(Polyline outerItem, TNative nativeItem);
        internal abstract void OnUpdateZIndex(Polyline outerItem, TNative nativeItem);
    }
}
