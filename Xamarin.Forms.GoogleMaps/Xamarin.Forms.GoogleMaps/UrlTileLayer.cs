using System;
namespace Xamarin.Forms.GoogleMaps
{
    public class UrlTileLayer : TileLayer
	{
		private Func<int, int, int, Uri> _makeTileUri;

		public UrlTileLayer(Func<int, int, int, Uri> makeTileUri, int tileSize=256) : base(tileSize)
		{
			_makeTileUri = makeTileUri;
		}

		protected UrlTileLayer(int tileSize = 256) : base(tileSize)
		{ 
			
		}

		public virtual Uri TileUri(int x, int y, int zoom) {
			return _makeTileUri(x, y, zoom);
		}
	}
}

