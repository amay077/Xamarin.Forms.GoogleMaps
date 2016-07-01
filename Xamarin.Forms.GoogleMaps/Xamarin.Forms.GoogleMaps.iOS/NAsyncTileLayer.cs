using System;
using Google.Maps;
using UIKit;
using System.Threading.Tasks;
using Foundation;
using ATileLayer = Google.Maps.TileLayer;

namespace Xamarin.Forms.GoogleMaps.iOS
{
	internal class NAsyncTileLayer : ATileLayer
	{
		private WeakReference _xformObject;

		public NAsyncTileLayer(AsyncTileLayer xformObject) : base()
		{
			_xformObject = new WeakReference(xformObject);
		}

		public override void RequestTile(nuint x, nuint y, nuint zoom, ITileReceiver receiver)
		{
			((AsyncTileLayer)_xformObject.Target).TileImageAsync((int)x, (int)y, (int)zoom).ContinueWith((Task<byte[]> task) => {
				var imgByte = task.Result;
				var image = new UIImage(NSData.FromArray(imgByte));
				receiver.ReceiveTile(x, y, zoom, image);
			});
		}
    }
}

