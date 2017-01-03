using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.GoogleMaps.Logics;
using Xamarin.Forms.GoogleMaps.UWP;

namespace Xamarin.Forms.GoogleMaps.Logics.UWP
{
    internal class TileLayerLogic : DefaultLogic<TileLayer, MapTileSource, MapControl>
    {
        protected override MapTileSource CreateNativeItem(TileLayer outerItem)
        {
            var nativeTileLayer = new MapTileSource();

            if (outerItem.MakeTileUri != null)
            {
                nativeTileLayer.DataSource = new UWPUriTileLayer(outerItem.MakeTileUri);
                nativeTileLayer.TilePixelSize = outerItem.TileSize;
                nativeTileLayer.AllowOverstretch = true;
                nativeTileLayer.Layer = MapTileLayer.BackgroundReplacement;
            }
            else if (outerItem.TileImageSync != null)
            {
                nativeTileLayer.DataSource = new UWPSyncTileLayer(outerItem.TileImageSync);
                nativeTileLayer.TilePixelSize = outerItem.TileSize;
                nativeTileLayer.AllowOverstretch = true;
                nativeTileLayer.Layer = MapTileLayer.BackgroundReplacement;
            }
            else
            {
                nativeTileLayer.DataSource = new UWPAsyncTileLayer(outerItem.TileImageAsync);
                nativeTileLayer.TilePixelSize = outerItem.TileSize;
                nativeTileLayer.AllowOverstretch = true;
                nativeTileLayer.Layer = MapTileLayer.BackgroundReplacement;
            }

            outerItem.NativeObject = nativeTileLayer;

            NativeMap.TileSources.Clear();
            NativeMap.TileSources.Add(nativeTileLayer);
            return nativeTileLayer;
        }

        protected override MapTileSource DeleteNativeItem(TileLayer outerItem)
        {
            var nativeTileLayer = outerItem.NativeObject as MapTileSource;
            if (nativeTileLayer == null)
                return null;

            NativeMap?.TileSources.Remove(nativeTileLayer);
            return nativeTileLayer;
        }

        protected override IList<TileLayer> GetItems(Map map)
        {
            return map.TileLayers;
        }
    }
}
