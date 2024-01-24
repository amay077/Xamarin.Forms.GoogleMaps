using UIKit;
using Foundation;
using NativeSyncTileLayer = Google.Maps.SyncTileLayer;

namespace Maui.GoogleMaps.iOS;

internal class TouchSyncTileLayer : NativeSyncTileLayer
{
    private readonly Func<int, int, int, byte[]> _tileImageSync;

    public TouchSyncTileLayer(Func<int, int, int, byte[]> tileImageSync) : base()
    {
        _tileImageSync = tileImageSync;
    }

    public override UIImage Tile(nuint x, nuint y, nuint zoom)
    {
        var imgByte = _tileImageSync((int)x, (int)y, (int)zoom);
        if (imgByte != null)
        {
            return new UIImage(NSData.FromArray(imgByte));
        }
        else
        {
            return null;
        }
    }
}