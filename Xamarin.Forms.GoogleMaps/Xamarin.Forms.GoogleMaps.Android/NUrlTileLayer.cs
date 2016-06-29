using System;
using Android.Gms.Maps.Model;

namespace Xamarin.Forms.GoogleMaps.Android
{
	public class NUrlTileLayer : UrlTileProvider
	{
		private static int tileWidth = 256;
		private static int tileHeight = 256;
		private Func<int, int, int, Uri> _makeTileUri;

		public NUrlTileLayer(Func<int, int, int, Uri> makeTileUri) : base(tileWidth, tileHeight)
		{
			_makeTileUri = makeTileUri;
		}

		public override Java.Net.URL GetTileUrl(int x, int y, int zoom)
		{
			var uri = _makeTileUri(x,y,zoom);
			return new Java.Net.URL(uri.AbsoluteUri);
		}
	}
}

