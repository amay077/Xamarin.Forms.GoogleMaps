using System;
using System.ComponentModel;
using Xamarin.Forms.GoogleMaps.Logics;
namespace Xamarin.Forms.GoogleMaps
{
    internal abstract class DefaultPinLogic<TNative, TNativeMap> : DefaultLogic<Pin, TNative, TNativeMap>
        where TNative : class
        where TNativeMap : class
    {
        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);
            var outerItem = sender as Pin;
            var nativeItem = outerItem?.NativeObject as TNative;

            if (nativeItem == null)
                return;

            if (e.PropertyName == Pin.AddressProperty.PropertyName) OnUpdateAddress(outerItem, nativeItem);
            else if (e.PropertyName == Pin.LabelProperty.PropertyName) OnUpdateLabel(outerItem, nativeItem);
            else if (e.PropertyName == Pin.PositionProperty.PropertyName) OnUpdatePosition(outerItem, nativeItem);
            else if (e.PropertyName == Pin.TypeProperty.PropertyName) OnUpdateType(outerItem, nativeItem);
            else if (e.PropertyName == Pin.IconProperty.PropertyName) OnUpdateIcon(outerItem, nativeItem);
            else if (e.PropertyName == Pin.IsDraggableProperty.PropertyName) OnUpdateIsDraggable(outerItem, nativeItem);
        }

        protected abstract void OnUpdateAddress(Pin outerItem, TNative nativeItem);

        protected abstract void OnUpdateLabel(Pin outerItem, TNative nativeItem);

        protected abstract void OnUpdatePosition(Pin outerItem, TNative nativeItem);

        protected abstract void OnUpdateType(Pin outerItem, TNative nativeItem);

        protected abstract void OnUpdateIcon(Pin outerItem, TNative nativeItem);

        protected abstract void OnUpdateIsDraggable(Pin outerItem, TNative nativeItem);
    }
}

