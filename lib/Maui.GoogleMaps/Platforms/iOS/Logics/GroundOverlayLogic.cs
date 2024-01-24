using Google.Maps;
using NativeGroundOverlay = Google.Maps.GroundOverlay;
using Maui.GoogleMaps.iOS.Extensions;
using Maui.GoogleMaps.iOS.Factories;

namespace Maui.GoogleMaps.Logics.iOS;

internal class GroundOverlayLogic : DefaultGroundOverlayLogic<NativeGroundOverlay, MapView>
{
    private readonly IImageFactory _imageFactory;

    public GroundOverlayLogic(IImageFactory imageFactory)
    {
        _imageFactory = imageFactory;
    }

    internal override void Register(MapView oldNativeMap, Map oldMap, MapView newNativeMap, Map newMap, IElementHandler handler)
    {
        base.Register(oldNativeMap, oldMap, newNativeMap, newMap, handler);

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

    protected override IList<GroundOverlay> GetItems(Map map) => map.GroundOverlays;

    protected override NativeGroundOverlay CreateNativeItem(GroundOverlay outerItem)
    {
        var nativeOverlay = NativeGroundOverlay.GetGroundOverlay(
            outerItem.Bounds.ToCoordinateBounds(),
            _imageFactory.ToUIImage(outerItem.Icon, Handler.MauiContext));
        nativeOverlay.Bearing = outerItem.Bearing;
        nativeOverlay.Opacity = 1 - outerItem.Transparency;
        nativeOverlay.Tappable = outerItem.IsClickable;
        nativeOverlay.ZIndex = outerItem.ZIndex;

        outerItem.NativeObject = nativeOverlay;
        nativeOverlay.Map = outerItem.IsVisible ? NativeMap : null;

        OnUpdateIconView(outerItem, nativeOverlay);

        return nativeOverlay;
    }

    protected override NativeGroundOverlay DeleteNativeItem(GroundOverlay outerItem)
    {
        var nativeOverlay = outerItem.NativeObject as NativeGroundOverlay;
        nativeOverlay.Map = null;

        return nativeOverlay;
    }

    void OnOverlayTapped(object sender, GMSOverlayEventEventArgs e)
    {
        var targetOuterItem = GetItems(Map).FirstOrDefault(
            outerItem => object.ReferenceEquals(outerItem.NativeObject, e.Overlay));
        targetOuterItem?.SendTap();
    }

    internal override void OnUpdateBearing(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.Bearing = outerItem.Bearing;
    }

    internal override void OnUpdateBounds(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.Bounds = outerItem.Bounds.ToCoordinateBounds();
    }

    internal override void OnUpdateIcon(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.Icon = _imageFactory.ToUIImage(outerItem.Icon, Handler.MauiContext);
    }

    internal override void OnUpdateIsClickable(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.Tappable = outerItem.IsClickable;
    }

    internal override void OnUpdateTransparency(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.Opacity = 1f - outerItem.Transparency;
    }

    internal override void OnUpdateZIndex(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.ZIndex = outerItem.ZIndex;
    }

    protected void OnUpdateIconView(GroundOverlay outerItem, NativeGroundOverlay nativeItem)
    {
        nativeItem.Icon = _imageFactory.ToUIImage(outerItem.Icon, Handler.MauiContext);
    }
}