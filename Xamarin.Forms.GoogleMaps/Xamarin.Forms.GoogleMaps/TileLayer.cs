﻿using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps
{
    public sealed class TileLayer : BindableObject
	{
		// For URI specific SyncTileLayer
        internal Func<int, int, int, Uri> MakeTileUri { get; }

		// For Image specific SyncTileLayer 
        internal Func<int, int, int, byte[]> TileImageSync { get; }

		// For Image specific AsyncTileLayer 
        internal Func<int, int, int, Task<byte[]>> TileImageAsync { get; }

        public int TileSize { get; } = 256;

		public object Tag { get; set; }

		public object NativeObject { get; internal set; }

		private TileLayer(Func<int, int, int, Uri> makeTileUri, int tileSize = 256)
		{
			this.MakeTileUri = makeTileUri;
			this.TileSize = tileSize;
		}

		private TileLayer(Func<int, int, int, byte[]> tileImageSync, int tileSize = 256)
		{
			this.TileImageSync = tileImageSync;
			this.TileSize = tileSize;
		}

		private TileLayer(Func<int, int, int, Task<byte[]>> tileImageAsync, int tileSize = 256)
		{
			this.TileImageAsync = tileImageAsync;
			this.TileSize = tileSize;
		}

		public static TileLayer FromTileUri(Func<int, int, int, Uri> makeTileUri, int tileSize = 256) {
			return new TileLayer(makeTileUri, tileSize);
		}

		public static TileLayer FromSyncImage(Func<int, int, int, byte[]> tileImageSync, int tileSize = 256)
		{
			return new TileLayer(tileImageSync, tileSize);
		}

		public static TileLayer FromAsyncImage(Func<int, int, int, Task<byte[]>> tileImageAsync, int tileSize = 256)
		{
			return new TileLayer(tileImageAsync, tileSize);
		}
	}
}

