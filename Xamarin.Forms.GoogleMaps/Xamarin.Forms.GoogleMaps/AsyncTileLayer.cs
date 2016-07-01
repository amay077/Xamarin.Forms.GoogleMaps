using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps
{
	public class AsyncTileLayer : TileLayer
	{
		private Func<int, int, int, Task<byte[]>> _tileImageAsync;

		public AsyncTileLayer(Func<int, int, int, Task<byte[]>> tileImageAsync, int tileSize = 256) : base(tileSize)
		{
			_tileImageAsync = tileImageAsync;
		}

		protected AsyncTileLayer(int tileSize = 256) : base(tileSize)
		{

		}

		public virtual Task<byte[]> TileImageAsync(int x, int y, int zoom)
		{
			return _tileImageAsync(x, y, zoom);
		}
	}
}

