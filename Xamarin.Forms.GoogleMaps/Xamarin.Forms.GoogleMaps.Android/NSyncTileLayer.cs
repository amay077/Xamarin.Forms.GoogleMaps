using System;
using Android.Gms.Maps.Model;
using IATileProvider = Android.Gms.Maps.Model.ITileProvider;

namespace Xamarin.Forms.GoogleMaps.Android
{
	public class NSyncTileLayer : Java.Lang.Object, IATileProvider
	{
		private Func<int, int, int, byte[]> _tileImageSync;
		private int _tileSize;

		public NSyncTileLayer(Func<int, int, int, byte[]> tileImageSync, int tileSize = 256) : base()
		{
			_tileImageSync = tileImageSync;
			_tileSize = tileSize;
		}

		public Tile GetTile(int x, int y, int zoom)
		{
			var imgByte = _tileImageSync(x, y, zoom);
			return new Tile(_tileSize, _tileSize, imgByte);
		}
	}
}