using System;
namespace Xamarin.Forms.GoogleMaps
{
	public class SyncTileLayer : TileLayer
	{
		private Func<int, int, int, byte[]> _tileImageSync;

		public SyncTileLayer(Func<int, int, int, byte[]> tileImageSync, int tileSize = 256) : base(tileSize)
		{
			_tileImageSync = tileImageSync;
		}

		protected SyncTileLayer(int tileSize = 256) : base(tileSize)
		{

		}

		public virtual byte[] TileImage(int x, int y, int zoom)
		{
			return _tileImageSync(x, y, zoom);
		}
	}
}

