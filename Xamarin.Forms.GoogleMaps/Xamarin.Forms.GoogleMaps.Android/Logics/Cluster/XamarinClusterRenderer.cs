using System.Collections.Generic;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Util;
using Com.Google.Maps.Android.Clustering;
using Com.Google.Maps.Android.Clustering.View;
using Xamarin.Forms.GoogleMaps.Android.Extensions;
using Xamarin.Forms.Platform.Android;
using NativeBitmapDescriptor = Android.Gms.Maps.Model.BitmapDescriptor;

namespace Xamarin.Forms.GoogleMaps.Android.Logics.Cluster
{
	public class XamarinClusterRenderer : DefaultClusterRenderer
	{
		private Map _map;
		private SparseArray<NativeBitmapDescriptor> _standardCache;
		private SparseArray<NativeBitmapDescriptor> _icons;
		private readonly float _density;

		public XamarinClusterRenderer(Context context, Map map, GoogleMap nativeMap, ClusterManager manager)
			: base(context, nativeMap, manager)
		{
			this._map = map;
			this._standardCache = new SparseArray<NativeBitmapDescriptor>();
			this._icons = new SparseArray<NativeBitmapDescriptor>();
		}

		protected override void OnBeforeClusterRendered(ICluster cluster, MarkerOptions options)
		{
			/* How we're drawing this? */
			if (this._map.ClusterOptions.RendererCallback != null)
			{
				/* We're using custom lambda callback */

				/* We're using custom image */
				if (this._map.ClusterOptions.EnableBuckets)
				{
					var bucketIndex = this.BucketIndexForSize(cluster.Size);
					var icon = this._icons.Get(bucketIndex);
					if (icon == null)
					{
						icon = this._map.ClusterOptions.RendererCallback(this.GetClusterText(cluster)).ToBitmapDescriptor();
						this._icons.Put(bucketIndex, icon);
					}

					options.SetIcon(icon);
				}
				else
				{
					var bucketIndex = this.BucketIndexForSize(cluster.Size);
					var icon = this._standardCache.Get(bucketIndex);
					if (icon == null)
					{
						icon = this._map.ClusterOptions.RendererCallback(cluster.Size.ToString()).ToBitmapDescriptor();
						this._standardCache.Put(bucketIndex, icon);
					}

					options.SetIcon(icon);
				}
			}
			else if (this._map.ClusterOptions.RendererImage != null)
			{
				/* We're using custom image */
				if (this._map.ClusterOptions.EnableBuckets)
				{
					var bucketIndex = this.BucketIndexForSize(cluster.Size);
					var icon = this._icons.Get(bucketIndex);
					if (icon == null)
					{
						icon = this._map.ClusterOptions.RendererImage.ToBitmapDescriptor();
						this._icons.Put(bucketIndex, icon);
					}

					options.SetIcon(icon);
				}
				else
				{
					var bucketIndex = this.BucketIndexForSize(cluster.Size);
					var icon = this._standardCache.Get(bucketIndex);
					if (icon == null)
					{
						icon = this._map.ClusterOptions.RendererImage.ToBitmapDescriptor();
						this._standardCache.Put(bucketIndex, icon);
					}

					options.SetIcon(icon);
				}
			}
			else
			{
				/* We can't access R directly here, so we'll just rely on the default implementation,
                which will use our buckets (see GetColor and GetBucket). */
				base.OnBeforeClusterRendered(cluster, options);
			}
		}

		protected override void OnBeforeClusterItemRendered(Java.Lang.Object marker, MarkerOptions options)
		{
			/* Fetch the clustered marker */
			var clusteredMarker = marker as ClusteredMarker;

			/* Fill the marker options from the clustered marker, the base method
             * will use these options to generate the pin, so we don't need to work hard here, yay! */
			options.SetTitle(clusteredMarker.Title)
				.SetSnippet(clusteredMarker.Snippet)
				.SetSnippet(clusteredMarker.Snippet)
				.Draggable(clusteredMarker.Draggable)
				.SetRotation(clusteredMarker.Rotation)
				.Anchor((float)clusteredMarker.AnchorX, (float)clusteredMarker.AnchorY)
				.Flat(clusteredMarker.Flat);

			/* Do we got an icon? */
			if (clusteredMarker.Icon != null)
			{
				options.SetIcon(clusteredMarker.Icon);
			}

			/* Shot! */
			base.OnBeforeClusterItemRendered(marker, options);
		}

		protected override int GetBucket(ICluster cluster)
		{
			return this._map.ClusterOptions.Buckets[this.BucketIndexForSize(cluster.Size)];
		}

		protected override int GetColor(int size)
		{
			return this._map.ClusterOptions.BucketColors[this.BucketIndexForSize(size)].ToAndroid();
		}

		private string GetClusterText(ICluster cluster)
		{
			string result = null;
			var size = cluster.Size;

			/* We're using buckets? */
			if (this._map.ClusterOptions.EnableBuckets)
			{
				var buckets = this._map.ClusterOptions.Buckets;
				var bucketIndex = this.BucketIndexForSize(size);
				// If size is smaller to first bucket size, use the size as is otherwise round it down to the
				// nearest bucket to limit the number of cluster icons we need to generate.

				if (size < buckets[0])
				{
					result = size.ToString();
				}
				else
				{
					result = $"{buckets[bucketIndex]}+";
				}
			}
			else
			{
				result = size.ToString();
			}

			return result;
		}

		private int BucketIndexForSize(int size)
		{
			// Finds the smallest bucket which is greater than |size|. If none exists return the last bucket
			// index (i.e |_buckets.count - 1|).
			uint index = 0;
			var buckets = this._map.ClusterOptions.Buckets;

			while (index + 1 < buckets.Length && buckets[index + 1] <= size)
			{
				++index;
			}

			return (int)index;
		}

	}
}