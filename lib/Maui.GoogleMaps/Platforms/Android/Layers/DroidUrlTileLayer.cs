using Android.Gms.Maps.Model;

namespace Maui.GoogleMaps.Android.Layers;

internal class DroidUrlTileLayer : UrlTileProvider
{
    private readonly Func<int, int, int, Uri> _makeTileUri;

    public DroidUrlTileLayer(Func<int, int, int, Uri> makeTileUri, int tileSize = 256) : base(tileSize, tileSize)
    {
        _makeTileUri = makeTileUri;
    }

    public override Java.Net.URL GetTileUrl(int x, int y, int zoom)
    {
        var uri = _makeTileUri(x, y, zoom);
        if (uri != null)
        {
            return new Java.Net.URL(uri.AbsoluteUri);
        }
        else
        {
            return null;
        }
    }
}