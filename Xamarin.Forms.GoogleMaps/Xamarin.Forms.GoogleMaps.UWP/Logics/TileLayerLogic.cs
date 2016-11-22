using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.GoogleMaps.Logics;

namespace Xamarin.Forms.GoogleMaps.Logics.UWP
{
    internal class TileLayerLogic : DefaultLogic<TileLayer, MapTileDataSource, MapControl>
    {
        protected override MapTileDataSource CreateNativeItem(TileLayer outerItem)
        {
            MapTileDataSource nativeTileLayer = new MapTileDataSource();

            //if (outerItem.MakeTileUri != null)
            //{
            //    nativeTileLayer = NativeUrlTileLayer.FromUrlConstructor((nuint x, nuint y, nuint zoom) =>
            //    {
            //        var uri = outerItem.MakeTileUri((int)x, (int)y, (int)zoom);
            //        return new NSUrl(uri.AbsoluteUri);
            //    });
            //    nativeTileLayer.TilePixelSize = outerItem.TileSize;
            //}
            //else if (outerItem.TileImageSync != null)
            //{
            //    nativeTileLayer = new TouchSyncTileLayer(outerItem.TileImageSync);
            //    nativeTileLayer.TileSize = (nint)outerItem.TileSize;
            //}
            //else
            //{
            //    nativeTileLayer = new TouchAsyncTileLayer(outerItem.TileImageAsync);
            //    nativeTileLayer.TileSize = (nint)outerItem.TileSize;
            //}

            //outerItem.NativeObject = nativeTileLayer;
            //nativeTileLayer.Map = NativeMap;

            return nativeTileLayer;
        }

        protected override MapTileDataSource DeleteNativeItem(TileLayer outerItem)
        {
            var nativeTileLayer = outerItem.NativeObject as MapTileDataSource;
            if (nativeTileLayer == null)
                return null;
            //nativeTileLayer.Remove();
            return nativeTileLayer;
        }

        protected override IList<TileLayer> GetItems(Map map)
        {
            return map.TileLayers;
        }
    }
}
