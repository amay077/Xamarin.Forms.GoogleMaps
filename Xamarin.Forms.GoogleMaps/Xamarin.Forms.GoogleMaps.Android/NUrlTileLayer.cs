using System;
using Android.Gms.Maps.Model;

namespace Xamarin.Forms.GoogleMaps.Android
{
	internal class NUrlTileLayer : UrlTileProvider
	{
		private WeakReference _xformObject;

		public NUrlTileLayer(UrlTileLayer xformObject, int tileSize = 256) : base(tileSize, tileSize)
		{
			_xformObject = new WeakReference(xformObject);
		}

		public override Java.Net.URL GetTileUrl(int x, int y, int zoom)
		{
			var uri = ((UrlTileLayer)_xformObject.Target).TileUri(x,y,zoom);
			return new Java.Net.URL(uri.AbsoluteUri);
		}
	}
}

