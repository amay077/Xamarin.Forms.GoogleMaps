using System.ComponentModel;
using Android.Gms.Maps.Model;
using NativeTileOverlay = Android.Gms.Maps.Model.TileOverlay;
using Android.Gms.Maps;
using Maui.GoogleMaps.Android.Layers;

namespace Maui.GoogleMaps.Logics.Android;

internal class TileLayerLogic : DefaultLogic<TileLayer, NativeTileOverlay, GoogleMap>
{
    protected override IList<TileLayer> GetItems(Map map) => map.TileLayers;

    protected override NativeTileOverlay CreateNativeItem(TileLayer outerItem)
    {
        var tileOverlayOptions = new TileOverlayOptions();

        var nativeTileProvider = CreateTileProvider();

        var nativeTileOverlay = NativeMap.AddTileOverlay(
            tileOverlayOptions.InvokeTileProvider(nativeTileProvider)
                .InvokeZIndex(outerItem.ZIndex));

        // associate pin with marker for later lookup in event handlers
        outerItem.NativeObject = nativeTileOverlay;
        return nativeTileOverlay;

        ITileProvider CreateTileProvider()
        {
            if (outerItem.MakeTileUri != null)
            {
                return new DroidUrlTileLayer(outerItem.MakeTileUri, outerItem.TileSize);
            }
            else if (outerItem.TileImageSync != null)
            {
                return new DroidSyncTileLayer(outerItem.TileImageSync, outerItem.TileSize);
            }

            return new DroidAsyncTileLayer(outerItem.TileImageAsync, outerItem.TileSize);
        }
    }

    protected override void CheckCanCreateNativeItem(TileLayer outerItem)
    {
    }

    protected override NativeTileOverlay DeleteNativeItem(TileLayer outerItem)
    {
        if (outerItem.NativeObject is not NativeTileOverlay nativeTileOverlay)
        {
            return null;
        }

        nativeTileOverlay.Remove();
        return nativeTileOverlay;
    }

    protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnItemPropertyChanged(sender, e);

        if (e.PropertyName != TileLayer.ZIndexProperty.PropertyName)
        {
            return;
        }

        if (sender is TileLayer { NativeObject: NativeTileOverlay nativeTileOverlay } tileLayer)
        {
            OnUpdateZIndex(tileLayer, nativeTileOverlay);
        }
    }

    private void OnUpdateZIndex(TileLayer outerItem, NativeTileOverlay nativeItem)
    {
        nativeItem.ZIndex = outerItem.ZIndex;
    }
}