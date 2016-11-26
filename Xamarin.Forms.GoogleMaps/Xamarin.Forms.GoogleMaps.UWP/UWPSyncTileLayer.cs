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
    internal class UWPSyncTileLayer : CustomMapTileDataSource
    {
        private readonly Func<int, int, int, byte[]> _makeTileUri;

        public UWPSyncTileLayer(Func<int, int, int, byte[]> makeTileUri, int tileSize = 256)
        {
            _makeTileUri = makeTileUri;
            this.BitmapRequested += UWPSyncTileLayer_BitmapRequested;
        }

        private void UWPSyncTileLayer_BitmapRequested(CustomMapTileDataSource sender, MapTileBitmapRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();
            var data = _makeTileUri(args.X, args.Y, args.ZoomLevel);

            if (data != null)
            {
                InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
                IOutputStream outputStream = randomAccessStream.GetOutputStreamAt(0);
                DataWriter writer = new DataWriter(outputStream);
                writer.WriteBytes(data);
                writer.StoreAsync().GetResults();
                writer.FlushAsync().GetResults();
                args.Request.PixelData = RandomAccessStreamReference.CreateFromStream(randomAccessStream);
                //MemoryStream stream = new MemoryStream();
                //stream.Write(data, 0, data.Length);
                //stream.Position = 0;
                //var streamReference = RandomAccessStreamReference.CreateFromStream(stream.AsRandomAccessStream());
                //args.Request.PixelData = streamReference; 
            }
            deferral.Complete();
        }
    }
}
