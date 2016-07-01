using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps
{
	public class TileLayer
	{
		// For URI specific SyncTileLayer
		private Func<int, int, int, Uri> _makeTileUri = null;
		internal Func<int, int, int, Uri> MakeTileUri 
		{
			get { return _makeTileUri; }
		}

		// For Image specific SyncTileLayer 
		private Func<int, int, int, byte[]> _tileImageSync = null;
		internal Func<int, int, int, byte[]> TileImageSync
		{
			get { return _tileImageSync; }
		}

		// For Image specific AsyncTileLayer 
		private Func<int, int, int, Task<byte[]>> _tileImageAsync = null;
		internal Func<int, int, int, Task<byte[]>> TileImageAsync
		{
			get { return _tileImageAsync; }
		}

		private int _tileSize = 256;
		public int TileSize
		{ 
			get { return _tileSize; }
		}

		public object Tag { get; set; }

		internal object Id { get; set; }

		private TileLayer(Func<int, int, int, Uri> makeTileUri, int tileSize = 256)
		{
			_makeTileUri = makeTileUri;
			_tileSize = tileSize;
		}

		private TileLayer(Func<int, int, int, byte[]> tileImageSync, int tileSize = 256)
		{
			_tileImageSync = tileImageSync;
			_tileSize = tileSize;
		}

		private TileLayer(Func<int, int, int, Task<byte[]>> tileImageAsync, int tileSize = 256)
		{
			_tileImageAsync = tileImageAsync;
			_tileSize = tileSize;
		}

		public static TileLayer FromTileUri(Func<int, int, int, Uri> makeTileUri, int tileSize = 256) {
			return new TileLayer(makeTileUri, tileSize);
		}

		public static TileLayer FromSyncImage(Func<int, int, int, byte[]> tileImageSync, int tileSize = 256)
		{
			return new TileLayer(tileImageSync, tileSize);
		}

		public static TileLayer FromASyncImage(Func<int, int, int, Task<byte[]>> tileImageAsync, int tileSize = 256)
		{
			return new TileLayer(tileImageAsync, tileSize);
		}
	}
}

