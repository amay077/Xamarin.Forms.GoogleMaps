using System;
using System.Threading.Tasks;
using Android.Gms.Maps.Model;
using INativeTileProvider = Android.Gms.Maps.Model.ITileProvider;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class NativeAsyncTileLayer : Java.Lang.Object, INativeTileProvider
	{
		private Func<int, int, int, Task<byte[]>> _tileImageAsync;
		private int _tileSize;

		public NativeAsyncTileLayer(Func<int, int, int, Task<byte[]>> tileImageAsync, int tileSize = 256) : base()
		{
			_tileImageAsync = tileImageAsync;
			_tileSize = tileSize;
		}

		public Tile GetTile(int x, int y, int zoom)
		{
			var imgByte = _tileImageAsync(x, y, zoom).Result;
			return new Tile(_tileSize, _tileSize, imgByte);
		}
	}
}

