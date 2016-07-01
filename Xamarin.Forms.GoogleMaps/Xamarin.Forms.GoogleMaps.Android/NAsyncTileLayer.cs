using System;
using System.Threading.Tasks;
using Android.Gms.Maps.Model;
using IATileProvider = Android.Gms.Maps.Model.ITileProvider;

namespace Xamarin.Forms.GoogleMaps.Android
{
    internal class NAsyncTileLayer : Java.Lang.Object, IATileProvider
	{
		private Func<int, int, int, Task<byte[]>> _tileImageAsync;
		private int _tileSize;

		public NAsyncTileLayer(Func<int, int, int, Task<byte[]>> tileImageAsync, int tileSize = 256) : base()
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

