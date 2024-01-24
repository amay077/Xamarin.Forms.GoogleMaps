using Android.Gms.Maps.Model;
using INativeTileProvider = Android.Gms.Maps.Model.ITileProvider;

namespace Maui.GoogleMaps.Android.Layers;

internal class DroidSyncTileLayer : Java.Lang.Object, INativeTileProvider
{
    private readonly Func<int, int, int, byte[]> _tileImageSync;
    private readonly int _tileSize;

    public DroidSyncTileLayer(Func<int, int, int, byte[]> tileImageSync, int tileSize = 256) : base()
    {
        _tileImageSync = tileImageSync;
        _tileSize = tileSize;
    }

    public Tile GetTile(int x, int y, int zoom)
    {
        var imgByte = _tileImageSync(x, y, zoom);
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