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
                //NativeMap.Style = MapStyle.None;
            }
            else if (outerItem.TileImageSync != null)
            {
                nativeTileLayer.DataSource = new UWPSyncTileLayer(outerItem.TileImageSync);
                nativeTileLayer.TilePixelSize = outerItem.TileSize;
                nativeTileLayer.AllowOverstretch = true;
               // NativeMap.Style = MapStyle.None;
            }
            else
            {
                nativeTileLayer.DataSource = new UWPAsyncTileLayer(outerItem.TileImageAsync);
                nativeTileLayer.TilePixelSize = outerItem.TileSize;
                nativeTileLayer.AllowOverstretch = true;
                //NativeMap.Style = MapStyle.None;
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
            if (NativeMap != null)
            {
                NativeMap.TileSources.Remove(nativeTileLayer);
               // UpdateMapType();
            }
            return nativeTileLayer;
        }

        protected override IList<TileLayer> GetItems(Map map)
        {
            return map.TileLayers;
        }

        void UpdateMapType()
        {
            switch (Map.MapType)
            {
                case MapType.Street:
                    NativeMap.Style = MapStyle.Road;
                    break;
                case MapType.Satellite:
                    NativeMap.Style = MapStyle.Aerial;
                    break;
                case MapType.Hybrid:
                    NativeMap.Style = MapStyle.AerialWithRoads;
                    break;
                case MapType.None:
                    NativeMap.Style = MapStyle.None;
                    break;
            }
        }
    }
}
