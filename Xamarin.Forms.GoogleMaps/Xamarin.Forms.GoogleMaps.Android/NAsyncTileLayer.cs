using System;
using System.Threading.Tasks;
using Android.Gms.Maps.Model;
using IATileProvider = Android.Gms.Maps.Model.ITileProvider;

namespace Xamarin.Forms.GoogleMaps.Android
{
    internal class NAsyncTileLayer : Java.Lang.Object, IATileProvider
	{
		private WeakReference _xformObject;
		private int _tileSize;

		public NAsyncTileLayer(AsyncTileLayer xformObject, int tileSize = 256) : base()
		{
			_xformObject = new WeakReference(xformObject);
			_tileSize = tileSize;
		}

		public Tile GetTile(int x, int y, int zoom)
		{
			var imgByte = ((AsyncTileLayer)_xformObject.Target).TileImageAsync(x, y, zoom).Result;
			return new Tile(_tileSize, _tileSize, imgByte);
		}
	}
}

