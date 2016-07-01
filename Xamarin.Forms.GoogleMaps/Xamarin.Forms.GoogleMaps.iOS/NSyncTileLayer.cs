using System;
using Google.Maps;
using UIKit;
using System.Drawing;
using Foundation;
using ASyncTileLayer = Google.Maps.SyncTileLayer;

namespace Xamarin.Forms.GoogleMaps.iOS
{
	public class NSyncTileLayer : ASyncTileLayer
	{
		private Func<int, int, int, byte[]> _tileImageSync;

		public NSyncTileLayer(Func<int, int, int, byte[]> tileImageSync) : base()
		{
			_tileImageSync = tileImageSync;
		}

		public override UIImage Tile(nuint x, nuint y, nuint zoom)
		{
			var imgByte = _tileImageSync((int)x, (int)y, (int)zoom);
			return new UIImage(NSData.FromArray(imgByte));
		}
	}
}