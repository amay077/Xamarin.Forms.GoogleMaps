using System;
using UIKit;
using Foundation;
using ASyncTileLayer = Google.Maps.SyncTileLayer;

namespace Xamarin.Forms.GoogleMaps.iOS
{
	internal class NSyncTileLayer : ASyncTileLayer
	{
		private WeakReference _xformObject;

		public NSyncTileLayer(SyncTileLayer xformObject) : base()
		{
			_xformObject = new WeakReference(xformObject);
		}

		public override UIImage Tile(nuint x, nuint y, nuint zoom)
		{
			var imgByte = ((SyncTileLayer)_xformObject.Target).TileImage((int)x, (int)y, (int)zoom);
			return new UIImage(NSData.FromArray(imgByte));
		}
	}
}