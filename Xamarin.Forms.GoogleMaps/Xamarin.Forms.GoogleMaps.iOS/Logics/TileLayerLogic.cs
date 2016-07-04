using System;
using System.Collections.Generic;
using System.ComponentModel;
using Foundation;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.Logics.iOS;
using NativeTileLayer = Google.Maps.TileLayer;
using NativeUrlTileLayer = Google.Maps.UrlTileLayer;

namespace Xamarin.Forms.GoogleMaps.iOS
{
    internal class TileLayerLogic : DefaultLogic<TileLayer, NativeTileLayer>
    {
        protected override IList<TileLayer> GetItems(Map map) => map.TileLayers;

        protected override NativeTileLayer CreateNativeItem(TileLayer outerItem)
        {
            NativeTileLayer nativeTileLayer;

            if (outerItem.MakeTileUri != null)
            {
                nativeTileLayer = NativeUrlTileLayer.FromUrlConstructor((nuint x, nuint y, nuint zoom) =>
                {
                    var uri = outerItem.MakeTileUri((int)x, (int)y, (int)zoom);
                    return new NSUrl(uri.AbsoluteUri);
                });
                nativeTileLayer.TileSize = (nint)outerItem.TileSize;
            }
            else if (outerItem.TileImageSync != null)
            {
                nativeTileLayer = new NativeSyncTileLayer(outerItem.TileImageSync);
                nativeTileLayer.TileSize = (nint)outerItem.TileSize;
            }
            else
            {
                nativeTileLayer = new NativeAsyncTileLayer(outerItem.TileImageAsync);
                nativeTileLayer.TileSize = (nint)outerItem.TileSize;
            }

            outerItem.NativeObject = nativeTileLayer;
            nativeTileLayer.Map = NativeMap;

            return nativeTileLayer;
        }

        protected override NativeTileLayer DeleteNativeItem(TileLayer outerItem)
        {
            var nativeTileLayer = outerItem.NativeObject as NativeTileLayer;
            nativeTileLayer.Map = null;
            return nativeTileLayer;
        }

        internal override void OnElementPropertyChanged(PropertyChangedEventArgs e)
        {
        }
    }
}

