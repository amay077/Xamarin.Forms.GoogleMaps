using Google.Maps;
using UIKit;
using Foundation;
using ATileLayer = Google.Maps.TileLayer;

namespace Maui.GoogleMaps.iOS;

internal class TouchAsyncTileLayer : ATileLayer
{
    private readonly Func<int, int, int, Task<byte[]>> _tileImageAsync;

    public TouchAsyncTileLayer(Func<int, int, int, Task<byte[]>> tileImageAsync) : base()
    {
        _tileImageAsync = tileImageAsync;
    }

    public override void RequestTile(nuint x, nuint y, nuint zoom, ITileReceiver receiver)
    {
        _tileImageAsync((int)x, (int)y, (int)zoom).ContinueWith((Task<byte[]> task) =>
        {
            var imgByte = task.Result;
            if (imgByte != null)
            {
                var image = new UIImage(NSData.FromArray(imgByte));
                receiver.ReceiveTile(x, y, zoom, image);
            }
        });
    }
}