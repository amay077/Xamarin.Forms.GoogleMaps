using System;
using Android.Gms.Maps.Model;
using INativeTileProvider = Android.Gms.Maps.Model.ITileProvider;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
	internal class NativeSyncTileLayer : Java.Lang.Object, INativeTileProvider
	{
		private readonly Func<int, int, int, byte[]> _tileImageSync;
		private readonly int _tileSize;

		public NativeSyncTileLayer(Func<int, int, int, byte[]> tileImageSync, int tileSize = 256) : base()
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