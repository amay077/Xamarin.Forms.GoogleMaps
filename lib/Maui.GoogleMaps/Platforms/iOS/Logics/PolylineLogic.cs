using Google.Maps;
using Microsoft.Maui.Platform;
using Maui.GoogleMaps.iOS.Extensions;
using NativePolyline = Google.Maps.Polyline;

namespace Maui.GoogleMaps.Logics.iOS;

internal class PolylineLogic : DefaultPolylineLogic<NativePolyline, MapView>
{
    internal override void Register(MapView oldNativeMap, Map oldMap, MapView newNativeMap, Map newMap, IElementHandler handler)
    {
        base.Register(oldNativeMap,oldMap, newNativeMap, newMap, handler);

        if (newNativeMap != null)
        {
            newNativeMap.OverlayTapped += OnOverlayTapped;
        }
    }

    internal override void Unregister(MapView nativeMap, Map map)
    {
        if (nativeMap != null)
        {
            nativeMap.OverlayTapped -= OnOverlayTapped;
        }

        base.Unregister(nativeMap, map);
    }

    protected override IList<Polyline> GetItems(Map map) => map.Polylines;

    protected override NativePolyline CreateNativeItem(Polyline outerItem)
    {
        var path = outerItem.Positions.ToMutablePath();
        var nativePolyline = NativePolyline.FromPath(path);
        nativePolyline.StrokeWidth = outerItem.StrokeWidth;
        nativePolyline.StrokeColor = outerItem.StrokeColor.ToPlatform();
        nativePolyline.Tappable = outerItem.IsClickable;
        nativePolyline.ZIndex = outerItem.ZIndex;

        outerItem.NativeObject = nativePolyline;
        nativePolyline.Map = NativeMap;

        outerItem.SetOnPositionsChanged((polyline, e) =>
        {
            var native = polyline.NativeObject as NativePolyline;
            native.Path = polyline.Positions.ToMutablePath();
        });

        return nativePolyline;
    }

    protected override NativePolyline DeleteNativeItem(Polyline outerItem)
    {
        var nativePolyline = outerItem.NativeObject as NativePolyline;
        nativePolyline.Map = null;
        return nativePolyline;
    }

    void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
    {
        var targetOuterItem = GetItems(Map).FirstOrDefault(
            outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
        targetOuterItem?.SendTap();
    }

    internal override void OnUpdateIsClickable(Polyline outerItem, NativePolyline nativeItem)
    {
        nativeItem.Tappable = outerItem.IsClickable;
    }

    internal override void OnUpdateStrokeColor(Polyline outerItem, NativePolyline nativeItem)
    {
        nativeItem.StrokeColor = outerItem.StrokeColor.ToPlatform();
    }

    internal override void OnUpdateStrokeWidth(Polyline outerItem, NativePolyline nativeItem)
    {
        nativeItem.StrokeWidth = outerItem.StrokeWidth;
    }

    internal override void OnUpdateZIndex(Polyline outerItem, NativePolyline nativeItem)
    {
        nativeItem.ZIndex = outerItem.ZIndex;
    }
}