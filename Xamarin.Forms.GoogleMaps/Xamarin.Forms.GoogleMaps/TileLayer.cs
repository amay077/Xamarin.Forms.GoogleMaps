using System;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps
{
    public abstract class TileLayer
	{
        public int TileSize { get; }

		public object Tag { get; set; }

		internal object NativeObject { get; set; }

		protected TileLayer(int tileSize = 256)
		{
			this.TileSize = tileSize;
		}
	}
}

