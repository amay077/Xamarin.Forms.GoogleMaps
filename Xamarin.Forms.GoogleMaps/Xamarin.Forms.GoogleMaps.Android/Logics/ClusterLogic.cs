using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Gms.Maps;
using System.Linq;
using System.ComponentModel;
using Xamarin.Forms.GoogleMaps.Android;
using Xamarin.Forms.GoogleMaps.Android.Extensions;
using NativeBitmapDescriptorFactory = Android.Gms.Maps.Model.BitmapDescriptorFactory;
using Android.Widget;
using System;
using Com.Google.Maps.Android.Clustering;
using Com.Google.Maps.Android.Clustering.Algo;
using Xamarin.Forms.GoogleMaps.Android.Logics.Cluster;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
	internal class ClusterLogic : DefaultPinLogic<ClusteredMarker, GoogleMap>
	{
		protected override IList<Pin> GetItems(Map map) => map.ClusteredPins;

		private volatile bool _onMarkerEvent = false;
		private Pin _draggingPin;
		private volatile bool _withoutUpdateNative = false;

		private ClusterManager _clusterManager;
		private ClusterLogicHandler _clusterHandler;

		internal override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
		{
			//----------------------------------------
			//  Base registration
			//----------------------------------------
			base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

			//----------------------------------------
			//  Instanciate a new cluster manager
			//----------------------------------------

			/* Initialize the manager */
			this._clusterManager = new ClusterManager(Xamarin.Forms.Forms.Context, this.NativeMap);
			this._clusterHandler = new ClusterLogicHandler(this.Map, this._clusterManager, this);

			/* Choose the right algorithm */
			switch (this.Map.ClusterOptions.Algorithm)
			{
				case ClusterAlgorithm.GridBased:
					this._clusterManager.SetAlgorithm(new GridBasedAlgorithm());
					break;
				case ClusterAlgorithm.NonHierarchicalDistanceBased:
				default:
					this._clusterManager.SetAlgorithm(new NonHierarchicalDistanceBasedAlgorithm());
					break;
			}

			//----------------------------------------
			//  Register
			//----------------------------------------
			if (newNativeMap != null)
			{
				/* Handle the cluster request by forwarding it to clusterManager.Cluster() */
				this.Map.OnCluster = HandleClusterRequest;

				this.NativeMap.SetOnCameraChangeListener(this._clusterManager);
				this.NativeMap.SetOnMarkerClickListener(this._clusterManager);
				//this.NativeMap.SetOnInfoWindowClickListener(this._clusterManager);

				this._clusterManager.SetRenderer(new XamarinClusterRenderer(Xamarin.Forms.Forms.Context, this.Map, this.NativeMap, this._clusterManager));

				this._clusterManager.SetOnClusterClickListener(this._clusterHandler);
				this._clusterManager.SetOnClusterInfoWindowClickListener(this._clusterHandler);
				this._clusterManager.SetOnClusterItemClickListener(this._clusterHandler);
				this._clusterManager.SetOnClusterItemInfoWindowClickListener(this._clusterHandler);
			}
		}

		internal override void Unregister(GoogleMap nativeMap, Map map)
		{
			if (nativeMap != null)
			{
				this.NativeMap.SetOnCameraChangeListener(null);
				this.NativeMap.SetOnMarkerClickListener(null);
				//this.NativeMap.SetOnInfoWindowClickListener(null);

				this._clusterHandler.Dispose();
				this._clusterManager.Dispose();
			}

			base.Unregister(nativeMap, map);
		}

		protected override ClusteredMarker CreateNativeItem(Pin outerItem)
		{
			var position = new LatLng(outerItem.Position.Latitude, outerItem.Position.Longitude);
			var marker = new ClusteredMarker(position);
			marker.Title = outerItem.Label;
			marker.Snippet = outerItem.Address;
			marker.Draggable = outerItem.IsDraggable;
			marker.Rotation = outerItem.Rotation;
			marker.AnchorX = (float)outerItem.Anchor.X;
			marker.AnchorY = (float)outerItem.Anchor.Y;
			marker.Flat = outerItem.Flat;

			outerItem.NativeObject = marker;

			if (outerItem.Icon != null)
			{
				marker.Icon = outerItem.Icon.ToBitmapDescriptor();
			}

			// If the pin has an IconView set this method will convert it into an icon for the marker
			if (outerItem?.Icon?.Type == BitmapDescriptorType.View)
			{
				marker.Visible = false; // Will become visible once the iconview is ready (see TransformXamarinViewToAndroidBitmap).

				TransformXamarinViewToAndroidBitmap(outerItem, marker);
			}
			else
			{
				marker.Visible = outerItem.IsVisible;
			}

			this._clusterManager.AddItem(marker);

			return marker;
		}

		protected override ClusteredMarker DeleteNativeItem(Pin outerItem)
		{
			var marker = outerItem.NativeObject as ClusteredMarker;
			if (marker == null)
				return null;
			this._clusterManager.RemoveItem(marker);
			outerItem.NativeObject = null;

			if (ReferenceEquals(Map.SelectedPin, outerItem))
				Map.SelectedPin = null;

			return marker;
		}

		internal Pin LookupPin(ClusteredMarker marker)
		{
			return GetItems(Map).FirstOrDefault(outerItem => ((ClusteredMarker)outerItem.NativeObject).Id == marker.Id);
		}

		protected override void AddItems(System.Collections.IList newItems)
		{
			base.AddItems(newItems);
		}

		public void HandleClusterRequest()
		{
			this._clusterManager.Cluster();
		}

		protected override void OnUpdateAddress(Pin outerItem, ClusteredMarker nativeItem)
			=> nativeItem.Snippet = outerItem.Address;

		protected override void OnUpdateLabel(Pin outerItem, ClusteredMarker nativeItem)
			=> nativeItem.Title = outerItem.Label;

		protected override void OnUpdatePosition(Pin outerItem, ClusteredMarker nativeItem)
		{
			if (!_withoutUpdateNative)
			{
				nativeItem.Position = outerItem.Position.ToLatLng();
			}
		}

		protected override void OnUpdateType(Pin outerItem, ClusteredMarker nativeItem)
		{
		}

		protected override void OnUpdateIcon(Pin outerItem, ClusteredMarker nativeItem)
		{
			if (outerItem.Icon != null && outerItem.Icon.Type == BitmapDescriptorType.View)
			{
				// If the pin has an IconView set this method will convert it into an icon for the marker
				TransformXamarinViewToAndroidBitmap(outerItem, nativeItem);
			}
			else
			{
				//nativeItem.SetIcon(outerItem?.Icon?.ToBitmapDescriptor() ?? NativeBitmapDescriptorFactory.DefaultMarker());
				//nativeItem.SetAnchor(0.5f, 1f);
				//nativeItem.SetInfoWindowAnchor(0.5f, 0f);
			}
		}

		protected override void OnUpdateIsDraggable(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.Draggable = outerItem?.IsDraggable ?? false;
		}

		protected override void OnUpdateRotation(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.Rotation = outerItem?.Rotation ?? 0f;
		}

		protected override void OnUpdateIsVisible(Pin outerItem, ClusteredMarker nativeItem)
		{
			var isVisible = outerItem?.IsVisible ?? false;
			nativeItem.Visible = isVisible;

			if (!isVisible && ReferenceEquals(Map.SelectedPin, outerItem))
			{
				Map.SelectedPin = null;
			}
		}

		protected override void OnUpdateAnchor(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.AnchorX = (float)outerItem.Anchor.X;
			nativeItem.AnchorY = (float)outerItem.Anchor.Y;
		}

		protected override void OnUpdateFlat(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.Flat = outerItem.Flat;
		}

		protected override void OnUpdateInfoWindowAnchor(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.AnchorX = (float)outerItem.Anchor.X;
			nativeItem.AnchorY = (float)outerItem.Anchor.Y;
		}

		protected override void OnUpdateZIndex(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.AnchorX = (float)outerItem.Anchor.X;
			nativeItem.AnchorY = (float)outerItem.Anchor.Y;
		}

		private async void TransformXamarinViewToAndroidBitmap(Pin outerItem, ClusteredMarker nativeItem)
		{
			if (outerItem?.Icon?.Type == BitmapDescriptorType.View && outerItem?.Icon?.View != null)
			{
				var iconView = outerItem.Icon.View;
				var nativeView = await Utils.ConvertFormsToNative(iconView, new Rectangle(0, 0, (double)Utils.DpToPx((float)iconView.WidthRequest), (double)Utils.DpToPx((float)iconView.HeightRequest)), Platform.Android.Platform.CreateRenderer(iconView));
				var otherView = new FrameLayout(nativeView.Context);
				nativeView.LayoutParameters = new FrameLayout.LayoutParams(Utils.DpToPx((float)iconView.WidthRequest), Utils.DpToPx((float)iconView.HeightRequest));
				otherView.AddView(nativeView);
				nativeItem.Icon = await Utils.ConvertViewToBitmapDescriptor(otherView);
				nativeItem.AnchorX = (float)iconView.AnchorX;
				nativeItem.AnchorY = (float)iconView.AnchorY;
				nativeItem.Visible = true;
			}
		}
	}
}