using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Gms.Maps.Model;
using NativeTileOverlay = Android.Gms.Maps.Model.TileOverlay;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class TileLayerLogic : DefaultLogic<TileLayer, NativeTileOverlay>
    {
        protected override IList<TileLayer> GetItems(Map map) => map.TileLayers;

        protected override NativeTileOverlay CreateNativeItem(TileLayer outerItem)
        {
            var opts = new TileOverlayOptions();

            ITileProvider nativeTileProvider;

            if (outerItem.MakeTileUri != null)
            {
                nativeTileProvider = new NativeUrlTileLayer(outerItem.MakeTileUri, outerItem.TileSize);
            }
            else if (outerItem.TileImageSync != null)
            {
                nativeTileProvider = new NativeSyncTileLayer(outerItem.TileImageSync, outerItem.TileSize);
            }
            else
            {
                nativeTileProvider = new NativeAsyncTileLayer(outerItem.TileImageAsync, outerItem.TileSize);
            }
            var nativeTileOverlay = NativeMap.AddTileOverlay(opts.InvokeTileProvider(nativeTileProvider));

            // associate pin with marker for later lookup in event handlers
            outerItem.NativeObject = nativeTileOverlay;
            return nativeTileOverlay;
        }

        protected override NativeTileOverlay DeleteNativeItem(TileLayer outerItem)
        {
            var nativeTileOverlay = outerItem.NativeObject as NativeTileOverlay;
            if (nativeTileOverlay == null)
                return null;
            nativeTileOverlay.Remove();
            return nativeTileOverlay;
        }

        internal override void OnElementPropertyChanged(PropertyChangedEventArgs e)
        {
        }
    }
}

