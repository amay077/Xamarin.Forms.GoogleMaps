using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;

namespace Xamarin.Forms.GoogleMaps.UWP
{
    internal class UWPAsyncTileLayer : CustomMapTileDataSource
    {
        private readonly Func<int, int, int, Task<byte[]>> _makeTileUri;

        public UWPAsyncTileLayer(Func<int, int, int, Task<byte[]>> makeTileUri, int tileSize = 256)
        {
            _makeTileUri = makeTileUri;
            this.BitmapRequested += UWPSyncTileLayer_BitmapRequested;
        }

        private void UWPSyncTileLayer_BitmapRequested(CustomMapTileDataSource sender, MapTileBitmapRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();
            var data = _makeTileUri(args.X, args.Y, args.ZoomLevel).Result;
            MemoryStream m = new MemoryStream(data);
            m.Position = 0;
            args.Request.PixelData = RandomAccessStreamReference.CreateFromStream(m.AsRandomAccessStream());
            deferral.Complete();
        }
    }
}
