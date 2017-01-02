using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls.Maps;
using System.Runtime.InteropServices.WindowsRuntime;

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

        private async void UWPSyncTileLayer_BitmapRequested(CustomMapTileDataSource sender, MapTileBitmapRequestedEventArgs args)
        {
            var deferral = args.Request.GetDeferral();
            var data = _makeTileUri(args.X, args.Y, args.ZoomLevel);

            if (data != null)
            {
                MemoryStream stream = new MemoryStream();
                stream.Write(data, 0, data.Length);
                stream.Flush();
                stream.Position = 0;

                var decoder = await Windows.Graphics.Imaging.BitmapDecoder.CreateAsync(stream.AsRandomAccessStream());
                var pixelProvider = await decoder.GetPixelDataAsync(Windows.Graphics.Imaging.BitmapPixelFormat.Rgba8, Windows.Graphics.Imaging.BitmapAlphaMode.Straight, new Windows.Graphics.Imaging.BitmapTransform(), Windows.Graphics.Imaging.ExifOrientationMode.RespectExifOrientation, Windows.Graphics.Imaging.ColorManagementMode.ColorManageToSRgb);
                var pixelData = pixelProvider.DetachPixelData();

                InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
                IOutputStream outputStream = randomAccessStream.GetOutputStreamAt(0);
                DataWriter writer = new DataWriter(outputStream);
                writer.WriteBytes(pixelData);
                var i = await writer.StoreAsync();
                var d = await writer.FlushAsync();
                args.Request.PixelData = RandomAccessStreamReference.CreateFromStream(randomAccessStream);
            }
            deferral.Complete();
        }
    }
}
