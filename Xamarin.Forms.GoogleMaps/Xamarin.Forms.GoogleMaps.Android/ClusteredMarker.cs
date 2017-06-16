using System;
using Android.Gms.Maps.Model;
using Com.Google.Maps.Android.Clustering;
using NativeBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;


namespace Xamarin.Forms.GoogleMaps.Android
{
	/* Note, we can't inherit Marker, like we done in iOS,
     * since we must inherit from Java.Lang.Object.
     * 
     * Thus, we'll just use the same marker code that exists in Marker. */
	public class ClusteredMarker : Java.Lang.Object, IClusterItem
	{
		public LatLng Position { get; set; }

		public float Alpha { get; set; }

		public bool Draggable { get; set; }

		public bool Flat { get; set; }

		public string Id { get; set; }

		public bool IsInfoWindowShown { get; set; }

		public float Rotation { get; set; }

		public string Snippet { get; set; }

		public string Title { get; set; }

		public bool Visible { get; set; }

		public float AnchorX { get; set; }

		public float AnchorY { get; set; }

		public NativeBitmapDescriptor Icon { get; set; }

		public ClusteredMarker(LatLng position)
		{
			this.Id = Guid.NewGuid().ToString();
			this.Position = position;
		}
	}
}