using Android.Gms.Maps.Model;
using INativeTileProvider = Android.Gms.Maps.Model.ITileProvider;

namespace Maui.GoogleMaps.Android.Layers;

internal class DroidAsyncTileLayer : Java.Lang.Object, INativeTileProvider
{
    private readonly Func<int, int, int, Task<byte[]>> _tileImageAsync;
    private readonly int _tileSize;

    public DroidAsyncTileLayer(Func<int, int, int, Task<byte[]>> tileImageAsync, int tileSize = 256) : base()
    {
        _tileImageAsync = tileImageAsync;
        _tileSize = tileSize;
    }

    public Tile GetTile(int x, int y, int zoom)
    {
        var imgByte = _tileImageAsync(x, y, zoom).Result;
        if (imgByte != null)
        {
            return new Tile(_tileSize, _tileSize, imgByte);
        }
        else
        {
            return null;
        }
    }
}