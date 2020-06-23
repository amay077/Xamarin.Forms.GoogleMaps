using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Gms.Maps.Model;
using Xamarin.Forms.GoogleMaps.Android;
using NativeTileOverlay = Android.Gms.Maps.Model.TileOverlay;
using Android.Gms.Maps;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class TileLayerLogic : DefaultLogic<TileLayer, NativeTileOverlay, GoogleMap>
    {
        protected override IList<TileLayer> GetItems(Map map) => map.TileLayers;

        protected override NativeTileOverlay CreateNativeItem(TileLayer outerItem)
        {
            var opts = new TileOverlayOptions();

            ITileProvider nativeTileProvider;

            if (outerItem.MakeTileUri != null)
            {
                nativeTileProvider = new DroidUrlTileLayer(outerItem.MakeTileUri, outerItem.TileSize);
            }
            else if (outerItem.TileImageSync != null)
            {
                nativeTileProvider = new DroidSyncTileLayer(outerItem.TileImageSync, outerItem.TileSize);
            }
            else
            {
                nativeTileProvider = new DroidAsyncTileLayer(outerItem.TileImageAsync, outerItem.TileSize);
            }
            var nativeTileOverlay = NativeMap.AddTileOverlay(
                opts.InvokeTileProvider(nativeTileProvider)
                    .InvokeZIndex(outerItem.ZIndex));

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

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);
            var outerItem = sender as TileLayer;
            var nativeItem = outerItem?.NativeObject as NativeTileOverlay;

            if (nativeItem == null)
                return;

            if (e.PropertyName == TileLayer.ZIndexProperty.PropertyName) OnUpdateZIndex(outerItem, nativeItem);
        }

        private void OnUpdateZIndex(TileLayer outerItem, NativeTileOverlay nativeItem)
        {
            nativeItem.ZIndex = outerItem.ZIndex;
        }
    }
}

