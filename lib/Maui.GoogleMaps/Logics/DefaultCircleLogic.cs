using System.ComponentModel;

namespace Maui.GoogleMaps.Logics;

internal abstract class DefaultCircleLogic<TNative, TNativeMap> : DefaultLogic<Circle, TNative, TNativeMap>
    where TNative : class
    where TNativeMap : class
{
    protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnItemPropertyChanged(sender, e);

        if (sender is not Circle { NativeObject: TNative nativeItem } outerItem)
        {
            return;
        }

        if (e.PropertyName == Circle.StrokeWidthProperty.PropertyName) OnUpdateStrokeWidth(outerItem, nativeItem);
        else if (e.PropertyName == Circle.StrokeColorProperty.PropertyName) OnUpdateStrokeColor(outerItem, nativeItem);
        else if (e.PropertyName == Circle.FillColorProperty.PropertyName) OnUpdateFillColor(outerItem, nativeItem);
        else if (e.PropertyName == Circle.CenterProperty.PropertyName) OnUpdateCenter(outerItem, nativeItem);
        else if (e.PropertyName == Circle.RadiusProperty.PropertyName) OnUpdateRadius(outerItem, nativeItem);
        else if (e.PropertyName == Circle.IsClickableProperty.PropertyName) OnUpdateIsClickable(outerItem, nativeItem);
        else if (e.PropertyName == Circle.ZIndexProperty.PropertyName) OnUpdateZIndex(outerItem, nativeItem);
    }

    protected override void CheckCanCreateNativeItem(Circle outerItem)
    {
        if (outerItem.Center == null || outerItem?.Radius == null || outerItem.Radius.Meters <= 0f)
        {
            throw new ArgumentException("Circle must have a center and radius");
        }
    }

    protected abstract void OnUpdateStrokeWidth(Circle outerItem, TNative nativeItem);

    protected abstract void OnUpdateStrokeColor(Circle outerItem, TNative nativeItem);

    protected abstract void OnUpdateFillColor(Circle outerItem, TNative nativeItem);

    protected abstract void OnUpdateCenter(Circle outerItem, TNative nativeItem);

    protected abstract void OnUpdateRadius(Circle outerItem, TNative nativeItem);

    protected abstract void OnUpdateIsClickable(Circle outerItem, TNative nativeItem);

    protected abstract void OnUpdateZIndex(Circle outerItem, TNative nativeItem);
}