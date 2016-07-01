using System;
using Android.Gms.Maps.Model;
using IATileProvider = Android.Gms.Maps.Model.ITileProvider;

namespace Xamarin.Forms.GoogleMaps.Android
{
	internal class NSyncTileLayer : Java.Lang.Object, IATileProvider
	{
		private WeakReference _xformObject;
		private int _tileSize;

		public NSyncTileLayer(SyncTileLayer xformObject, int tileSize = 256) : base()
		{
			_xformObject = new WeakReference(xformObject);
			_tileSize = tileSize;
		}

		public Tile GetTile(int x, int y, int zoom)
		{
			var imgByte = ((SyncTileLayer)_xformObject.Target).TileImage(x, y, zoom);
			return new Tile(_tileSize, _tileSize, imgByte);
		}
	}
}