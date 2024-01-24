using System.ComponentModel;

namespace Maui.GoogleMaps.Logics;

internal abstract class DefaultPolygonLogic<TNative, TNativeMap> : DefaultLogic<Polygon, TNative, TNativeMap>
    where TNative : class
    where TNativeMap : class
{
    protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnItemPropertyChanged(sender, e);

        if (sender is not  Polygon { NativeObject: TNative nativeItem } outerItem)
        {
            return;
        }

        if (e.PropertyName == Polygon.IsClickableProperty.PropertyName) OnUpdateIsClickable(outerItem, nativeItem);
        else if (e.PropertyName == Polygon.StrokeColorProperty.PropertyName) OnUpdateStrokeColor(outerItem, nativeItem);
        else if (e.PropertyName == Polygon.StrokeWidthProperty.PropertyName) OnUpdateStrokeWidth(outerItem, nativeItem);
        else if (e.PropertyName == Polygon.FillColorProperty.PropertyName) OnUpdateFillColor(outerItem, nativeItem);
        else if (e.PropertyName == Polygon.ZIndexProperty.PropertyName) OnUpdateZIndex(outerItem, nativeItem);
    }

    protected override void CheckCanCreateNativeItem(Polygon outerItem)
    {
        if (outerItem.Positions.Count < 3)
        {
            throw new ArgumentException("Polygon must have a 3 positions to be added to a map");
        }
    }

    internal abstract void OnUpdateIsClickable(Polygon outerItem, TNative nativeItem);
    internal abstract void OnUpdateStrokeColor(Polygon outerItem, TNative nativeItem);
    internal abstract void OnUpdateStrokeWidth(Polygon outerItem, TNative nativeItem);
    internal abstract void OnUpdateFillColor(Polygon outerItem, TNative nativeItem);
    internal abstract void OnUpdateZIndex(Polygon outerItem, TNative nativeItem);
}