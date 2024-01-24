using System.ComponentModel;

namespace Maui.GoogleMaps.Logics;

internal abstract class DefaultPolylineLogic<TNative, TNativeMap> : DefaultLogic<Polyline, TNative, TNativeMap>
    where TNative : class
    where TNativeMap : class
{
    protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnItemPropertyChanged(sender, e);
        
        if (sender is not Polyline { NativeObject: TNative nativeItem } outerItem)
        {
            return;
        }

        if (e.PropertyName == Polyline.IsClickableProperty.PropertyName) OnUpdateIsClickable(outerItem, nativeItem);
        else if (e.PropertyName == Polyline.StrokeColorProperty.PropertyName) OnUpdateStrokeColor(outerItem, nativeItem);
        else if (e.PropertyName == Polyline.StrokeWidthProperty.PropertyName) OnUpdateStrokeWidth(outerItem, nativeItem);
        else if (e.PropertyName == Polyline.ZIndexProperty.PropertyName) OnUpdateZIndex(outerItem, nativeItem);
    }

    protected override void CheckCanCreateNativeItem(Polyline outerItem)
    {
        if (outerItem.Positions.Count < 2)
        {
            throw new ArgumentException("Polyline must have a 2 positions to be added to a map");
        }
    }

    internal abstract void OnUpdateIsClickable(Polyline outerItem, TNative nativeItem);
    internal abstract void OnUpdateStrokeColor(Polyline outerItem, TNative nativeItem);
    internal abstract void OnUpdateStrokeWidth(Polyline outerItem, TNative nativeItem);
    internal abstract void OnUpdateZIndex(Polyline outerItem, TNative nativeItem);
}