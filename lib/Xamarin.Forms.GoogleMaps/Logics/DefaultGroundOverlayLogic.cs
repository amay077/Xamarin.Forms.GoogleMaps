using System;
using System.ComponentModel;

namespace Xamarin.Forms.GoogleMaps.Logics
{
    internal abstract class DefaultGroundOverlayLogic<TNative, TNativeMap> : DefaultLogic<GroundOverlay, TNative, TNativeMap>
        where TNative : class
        where TNativeMap : class
    {
        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);
            var outerItem = sender as GroundOverlay;
            var nativeItem = outerItem?.NativeObject as TNative;

            if (nativeItem == null)
                return;

            if (e.PropertyName == GroundOverlay.BearingProperty.PropertyName) OnUpdateBearing(outerItem, nativeItem);
            else if (e.PropertyName == GroundOverlay.BoundsProperty.PropertyName) OnUpdateBounds(outerItem, nativeItem);
            else if (e.PropertyName == GroundOverlay.IconProperty.PropertyName) OnUpdateIcon(outerItem, nativeItem);
            else if (e.PropertyName == GroundOverlay.IsClickableProperty.PropertyName) OnUpdateIsClickable(outerItem, nativeItem);
            else if (e.PropertyName == GroundOverlay.TransparencyProperty.PropertyName) OnUpdateTransparency(outerItem, nativeItem);
            else if (e.PropertyName == GroundOverlay.ZIndexProperty.PropertyName) OnUpdateZIndex(outerItem, nativeItem);
        }

        internal abstract void OnUpdateBearing(GroundOverlay outerItem, TNative nativeItem);
        internal abstract void OnUpdateBounds(GroundOverlay outerItem, TNative nativeItem);
        internal abstract void OnUpdateIcon(GroundOverlay outerItem, TNative nativeItem);
        internal abstract void OnUpdateIsClickable(GroundOverlay outerItem, TNative nativeItem);
        internal abstract void OnUpdateTransparency(GroundOverlay outerItem, TNative nativeItem);
        internal abstract void OnUpdateZIndex(GroundOverlay outerItem, TNative nativeItem);
    }
}
