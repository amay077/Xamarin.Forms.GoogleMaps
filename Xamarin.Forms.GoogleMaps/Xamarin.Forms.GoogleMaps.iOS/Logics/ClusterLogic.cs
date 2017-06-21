using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using CoreLocation;
using Foundation;
using GMCluster;
using Google.Maps;
using UIKit;
using Xamarin.Forms.GoogleMaps.iOS;
using Xamarin.Forms.GoogleMaps.iOS.Extensions;
using Xamarin.Forms.Platform.iOS;

namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
	internal class ClusterLogic : DefaultPinLogic<ClusteredMarker, MapView>
	{
		protected override IList<Pin> GetItems(Map map) => map.ClusteredPins;

		private GMUClusterManager _clusterManager;
		private GMUClusterDelegateHandler _clusterDelegate;

		private bool _onMarkerEvent;
		private Pin _draggingPin;
		private volatile bool _withoutUpdateNative = false;

		internal override void Register(MapView oldNativeMap, Map oldMap, MapView newNativeMap, Map newMap)
		{
			//----------------------------------------
			//  Base registration
			//----------------------------------------
			base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

			//----------------------------------------
			//  Instanciate a new cluster manager
			//----------------------------------------

			/* Delegate */
			this._clusterDelegate = new GMUClusterDelegateHandler(this);

			/* Algorithm */
			IGMUClusterAlgorithm algorithm;
			switch (newMap.ClusterOptions.Algorithm)
			{
				case ClusterAlgorithm.GridBased:
					algorithm = new GMUGridBasedClusterAlgorithm();
					break;
				default:
					algorithm = new GMUNonHierarchicalDistanceBasedAlgorithm();
					break;
			}

			/* Icon Generation */
			var iconGenerator = new GMUClusterIconGeneratorHandler(this.Map.ClusterOptions);

			/* Renderer */
			var clusterRenderer = new GMUClusterRendererHandler(newNativeMap, iconGenerator);
			clusterRenderer.WeakDelegate = this._clusterDelegate;

			/* The actual manager */
			this._clusterManager = new GMUClusterManager(newNativeMap, algorithm, clusterRenderer);

			this._clusterManager.SetDelegate(this._clusterDelegate, this._clusterDelegate);

			/* Handle the cluster request by forwarding it to clusterManager.Cluster() */
			this.Map.OnCluster = HandleClusterRequest;

			//----------------------------------------
			//  Register events events
			//----------------------------------------
			if (newNativeMap != null)
			{
				newNativeMap.InfoTapped += OnInfoTapped;
				newNativeMap.TappedMarker = HandleGMSTappedMarker;
				newNativeMap.InfoClosed += InfoWindowClosed;
				newNativeMap.DraggingMarkerStarted += DraggingMarkerStarted;
				newNativeMap.DraggingMarkerEnded += DraggingMarkerEnded;
				newNativeMap.DraggingMarker += DraggingMarker;
			}
		}

		internal override void Unregister(MapView nativeMap, Map map)
		{
			//----------------------------------------
			//  Unregister events
			//----------------------------------------
			if (nativeMap != null)
			{
				nativeMap.DraggingMarker -= DraggingMarker;
				nativeMap.DraggingMarkerEnded -= DraggingMarkerEnded;
				nativeMap.DraggingMarkerStarted -= DraggingMarkerStarted;
				nativeMap.InfoClosed -= InfoWindowClosed;
				nativeMap.TappedMarker = null;
				nativeMap.InfoTapped -= OnInfoTapped;
			}

			/* Remove cluster handler */
			this.Map.OnCluster = null;

			//----------------------------------------
			//  Base
			//----------------------------------------
			base.Unregister(nativeMap, map);
		}

		protected override ClusteredMarker CreateNativeItem(Pin outerItem)
		{
			var nativeMarker = new ClusteredMarker() { Position = outerItem.Position.ToCoord() };
			nativeMarker.Title = outerItem.Label;
			nativeMarker.Snippet = outerItem.Address ?? string.Empty;
			nativeMarker.Draggable = outerItem.IsDraggable;
			nativeMarker.Rotation = outerItem.Rotation;
			nativeMarker.GroundAnchor = new CGPoint(outerItem.Anchor.X, outerItem.Anchor.Y);
			nativeMarker.Flat = outerItem.Flat;

			if (outerItem.Icon != null)
			{
				nativeMarker.Icon = outerItem.Icon.ToUIImage();
			}

			outerItem.NativeObject = nativeMarker;

			// When using clustering, we don't set the pin directly.
			//nativeMarker.Map = outerItem.IsVisible ? NativeMap : null;

			this._clusterManager.AddItem(nativeMarker);

			OnUpdateIconView(outerItem, nativeMarker);

			return nativeMarker;
		}

		protected override ClusteredMarker DeleteNativeItem(Pin outerItem)
		{
			var nativeMarker = outerItem.NativeObject as ClusteredMarker;
			nativeMarker.Map = null;

			if (ReferenceEquals(Map.SelectedPin, outerItem))
				Map.SelectedPin = null;

			return nativeMarker;
		}

		internal override void OnMapPropertyChanged(PropertyChangedEventArgs e)
		{
			if (e.PropertyName == Map.SelectedPinProperty.PropertyName)
			{
				if (!_onMarkerEvent)
					UpdateSelectedPin(Map.SelectedPin);
				Map.SendSelectedPinChanged(Map.SelectedPin);
			}
		}

		void UpdateSelectedPin(Pin pin)
		{
			if (pin != null)
				NativeMap.SelectedMarker = (ClusteredMarker)pin.NativeObject;
			else
				NativeMap.SelectedMarker = null;
		}

		Pin LookupPin(Marker marker)
		{
			return GetItems(Map).FirstOrDefault(outerItem => ReferenceEquals(outerItem.NativeObject, marker));
		}

		void HandleClusterRequest()
		{
			this._clusterManager.Cluster();
		}

		#region Events Handling

		void OnInfoTapped(object sender, GMSMarkerEventEventArgs e)
		{
			// lookup pin
			var targetPin = LookupPin(e.Marker);

			// only consider event handled if a handler is present.
			// Else allow default behavior of displaying an info window.
			targetPin?.SendTap();

			if (targetPin != null)
			{
				Map.SendInfoWindowClicked(targetPin);
			}
		}

		bool HandleGMSTappedMarker(MapView mapView, Marker marker)
		{
			// lookup pin
			var targetPin = LookupPin(marker);

			// If set to PinClickedEventArgs.Handled = true in app codes,
			// then all pin selection controlling by app.
			if (Map.SendPinClicked(targetPin))
			{
				return true;
			}

			try
			{
				_onMarkerEvent = true;
				if (targetPin != null && !ReferenceEquals(targetPin, Map.SelectedPin))
					Map.SelectedPin = targetPin;
			}
			finally
			{
				_onMarkerEvent = false;
			}

			return false;
		}

		void InfoWindowClosed(object sender, GMSMarkerEventEventArgs e)
		{
			// lookup pin
			var targetPin = LookupPin(e.Marker);

			try
			{
				_onMarkerEvent = true;
				if (targetPin != null && ReferenceEquals(targetPin, Map.SelectedPin))
					Map.SelectedPin = null;
			}
			finally
			{
				_onMarkerEvent = false;
			}
		}

		void DraggingMarkerStarted(object sender, GMSMarkerEventEventArgs e)
		{
			// lookup pin
			_draggingPin = LookupPin(e.Marker);

			if (_draggingPin != null)
			{
				UpdatePositionWithoutMove(_draggingPin, e.Marker.Position.ToPosition());
				Map.SendPinDragStart(_draggingPin);
			}
		}

		void DraggingMarkerEnded(object sender, GMSMarkerEventEventArgs e)
		{
			if (_draggingPin != null)
			{
				UpdatePositionWithoutMove(_draggingPin, e.Marker.Position.ToPosition());
				Map.SendPinDragEnd(_draggingPin);
				_draggingPin = null;
			}
		}

		void DraggingMarker(object sender, GMSMarkerEventEventArgs e)
		{
			if (_draggingPin != null)
			{
				UpdatePositionWithoutMove(_draggingPin, e.Marker.Position.ToPosition());
				Map.SendPinDragging(_draggingPin);
			}
		}

		void UpdatePositionWithoutMove(Pin pin, Position position)
		{
			try
			{
				_withoutUpdateNative = true;
				pin.Position = position;
			}
			finally
			{
				_withoutUpdateNative = false;
			}
		}

		#endregion

		#region Misc

		protected override void OnUpdateAddress(Pin outerItem, ClusteredMarker nativeItem)
			=> nativeItem.Snippet = outerItem.Address;

		protected override void OnUpdateLabel(Pin outerItem, ClusteredMarker nativeItem)
			=> nativeItem.Title = outerItem.Label;

		protected override void OnUpdatePosition(Pin outerItem, ClusteredMarker nativeItem)
		{
			if (!_withoutUpdateNative)
			{
				nativeItem.Position = outerItem.Position.ToCoord();
			}
		}

		protected override void OnUpdateType(Pin outerItem, ClusteredMarker nativeItem)
		{
		}

		protected override void OnUpdateIcon(Pin outerItem, ClusteredMarker nativeItem)
		{
			if (outerItem.Icon.Type == BitmapDescriptorType.View)
			{
				OnUpdateIconView(outerItem, nativeItem);
			}
			else
			{
				nativeItem.Icon = outerItem?.Icon?.ToUIImage();
			}
		}

		protected override void OnUpdateIsDraggable(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.Draggable = outerItem?.IsDraggable ?? false;
		}

		protected void OnUpdateIconView(Pin outerItem, ClusteredMarker nativeItem)
		{
			if (outerItem?.Icon?.Type == BitmapDescriptorType.View && outerItem?.Icon?.View != null)
			{
				NativeMap.InvokeOnMainThread(() =>
				{
					var iconView = outerItem.Icon.View;
					var nativeView = Utils.ConvertFormsToNative(iconView, new CGRect(0, 0, iconView.WidthRequest, iconView.HeightRequest));
					nativeView.BackgroundColor = UIColor.Clear;
					nativeItem.GroundAnchor = new CGPoint(iconView.AnchorX, iconView.AnchorY);
					nativeItem.Icon = Utils.ConvertViewToImage(nativeView);

					// Would have been way cooler to do this instead, but surprisingly, we can't do this on Android:
					// nativeItem.IconView = nativeView;
				});
			}
		}

		protected override void OnUpdateRotation(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.Rotation = outerItem?.Rotation ?? 0f;
		}

		protected override void OnUpdateIsVisible(Pin outerItem, ClusteredMarker nativeItem)
		{
			if (outerItem?.IsVisible ?? false)
			{
				nativeItem.Map = NativeMap;
			}
			else
			{
				nativeItem.Map = null;
				if (ReferenceEquals(Map.SelectedPin, outerItem))
					Map.SelectedPin = null;
			}
		}

		protected override void OnUpdateAnchor(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.GroundAnchor = new CGPoint(outerItem.Anchor.X, outerItem.Anchor.Y);
		}

		protected override void OnUpdateFlat(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.Flat = outerItem.Flat;
		}

		protected override void OnUpdateInfoWindowAnchor(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.InfoWindowAnchor = new CGPoint(outerItem.Anchor.X, outerItem.Anchor.Y);
		}

		protected override void OnUpdateZIndex(Pin outerItem, ClusteredMarker nativeItem)
		{
			nativeItem.InfoWindowAnchor = new CGPoint(outerItem.Anchor.X, outerItem.Anchor.Y);
		}

		#endregion

		#region GMUDefaultClusterRenderer

		internal class GMUClusterRendererHandler : GMUDefaultClusterRenderer
		{
			private MapView _nativeMap;
			private const double kGMUAnimationDuration = 0.5; // Pins grouping and splitting animation speed in seconds.

			public GMUClusterRendererHandler(MapView mapView, IGMUClusterIconGenerator iconGenerator)
				: base(mapView, iconGenerator)
			{
				this._nativeMap = mapView;
			}


			[Export("markerWithPosition:from:userData:clusterIcon:animated:")]
			public Marker MarkerWithPosition(CLLocationCoordinate2D position, CLLocationCoordinate2D from,
				NSObject userData, UIImage clusterIcon,
				bool animated)
			{
				/* We can get into this method in two cases:
                    1) We're resolving a specific marker.
                            In this case, the userData will be ClusteredMarker.
                            This is the marker that we've already been initialized in CreateNativeMarker(Pin).
                    2) We're resolving a cluster marker.
                            This is a "group" marker that shows how much pins it group, or clusters.
                            In this case, we're using the clusterIcon that was generated for us previouslly. */

				Marker marker = null;
				var initialPosition = animated ? from : position;
				if (userData is ClusteredMarker)
				{
					/* A single pin */
					var clusteredMarker = userData as ClusteredMarker;
					marker = Marker.FromPosition(initialPosition);
					marker.Icon = clusteredMarker.Icon;
					marker.Title = clusteredMarker.Title;
					marker.Snippet = clusteredMarker.Snippet;
					marker.Draggable = clusteredMarker.Draggable;
					marker.Rotation = clusteredMarker.Rotation;
					marker.GroundAnchor = clusteredMarker.GroundAnchor;
					marker.Flat = clusteredMarker.Flat;
				}
				else
				{
					/* A group */
					marker = Marker.FromPosition(initialPosition);
					marker.Icon = clusterIcon;
					marker.GroundAnchor = new CGPoint(0.5, 0.5);
				}

				/* Misc */
				marker.ZIndex = 1;
				marker.Position = initialPosition;
				marker.UserData = userData;

				/* Assign the marker to the map */
				marker.Map = this._nativeMap;

				/* Animate? */
				if (animated)
				{
					CATransaction.Begin();
					CATransaction.AnimationDuration = kGMUAnimationDuration;
					marker.Layer.Latitude = position.Latitude;
					marker.Layer.Longitude = position.Longitude;
					CATransaction.Commit();
				}

				return marker;
			}
		}

		#endregion

		#region GMUDefaultClusterIconGenerator

		internal class GMUClusterIconGeneratorHandler : GMUDefaultClusterIconGenerator
		{
			private NSCache _iconCache;
			private ClusterOptions _options;

			public GMUClusterIconGeneratorHandler(ClusterOptions options)
			{
				this._iconCache = new NSCache();
				this._options = options;
			}

			public override UIImage IconForSize(nuint size)
			{
				/* Setup */
				string text = null;
				nuint bucketIndex = 0;

				/* We're using buckets? */
				if (this._options.EnableBuckets)
				{
					var buckets = this._options.Buckets;
					bucketIndex = this.BucketIndexForSize((nint)size);
					// If size is smaller to first bucket size, use the size as is otherwise round it down to the
					// nearest bucket to limit the number of cluster icons we need to generate.

					if (size < (nuint)buckets[0])
					{
						text = size.ToString();
					}
					else
					{
						text = $"{buckets[bucketIndex]}+";
					}
				}

				/* How we're drawing this? */
				if (this._options.RendererCallback != null)
				{
					/* We're using custom lambda callback */
					return this._options.RendererCallback(text).ToUIImage();
				}
				else if (this._options.RendererImage != null)
				{
					/* We're using custom image */
					return this.GetIconForText(text, this._options.RendererImage.ToUIImage());
				}
				else
				{
					/* We're using raw colors */
					return this.GetIconForText(text, bucketIndex);
				}
			}

			private nuint BucketIndexForSize(nint size)
			{
				// Finds the smallest bucket which is greater than |size|. If none exists return the last bucket
				// index (i.e |_buckets.count - 1|).
				uint index = 0;
				var buckets = this._options.Buckets;

				while (index + 1 < buckets.Length && buckets[index + 1] <= size)
				{
					++index;
				}

				return index;
			}

			private UIImage GetIconForText(string text, UIImage baseImage)
			{
				var nsText = new NSString(text);
				var icon = _iconCache.ObjectForKey(nsText);
				if (icon != null)
				{
					return (UIImage)icon;
				}

				var font = UIFont.BoldSystemFontOfSize(12);
				var size = baseImage.Size;
				UIGraphics.BeginImageContextWithOptions(size, false, 0.0f);
				baseImage.Draw(new CGRect(0, 0, size.Width, size.Height));
				var rect = new CGRect(0, 0, baseImage.Size.Width, baseImage.Size.Height);

				var paragraphStyle = NSMutableParagraphStyle.Default;
				var attributes = new UIStringAttributes(NSDictionary.FromObjectsAndKeys(
					objects: new NSObject[] { font, paragraphStyle, this._options.RendererTextColor.ToUIColor() },
					keys: new NSObject[] { UIStringAttributeKey.Font, UIStringAttributeKey.ParagraphStyle, UIStringAttributeKey.ForegroundColor }
				));

				var textSize = nsText.GetSizeUsingAttributes(attributes);
				var textRect = RectangleFExtensions.Inset(rect, (rect.Size.Width - textSize.Width) / 2,
					(rect.Size.Height - textSize.Height) / 2);
				nsText.DrawString(RectangleFExtensions.Integral(textRect), attributes);

				var newImage = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();

				_iconCache.SetObjectforKey(newImage, nsText);
				return newImage;
			}

			private UIImage GetIconForText(string text, nuint bucketIndex)
			{
				var nsText = new NSString(text);
				var icon = _iconCache.ObjectForKey(nsText);
				if (icon != null)
				{
					return (UIImage)icon;
				}

				var font = UIFont.BoldSystemFontOfSize(14);
				var paragraphStyle = NSParagraphStyle.Default;
				var dict = NSDictionary.FromObjectsAndKeys(
					objects: new NSObject[] { font, paragraphStyle, this._options.RendererTextColor.ToUIColor() },
					keys: new NSObject[] { UIStringAttributeKey.Font, UIStringAttributeKey.ParagraphStyle, UIStringAttributeKey.ForegroundColor }
				);
				var attributes = new UIStringAttributes(dict);


				var textSize = nsText.GetSizeUsingAttributes(attributes);
				var rectDimension = Math.Max(20, Math.Max(textSize.Width, textSize.Height)) + 3 * bucketIndex + 6;
				var rect = new CGRect(0.0f, 0.0f, rectDimension, rectDimension);

				UIGraphics.BeginImageContext(rect.Size);
				UIGraphics.BeginImageContextWithOptions(rect.Size, false, 0);

				// Background circle
				var ctx = UIGraphics.GetCurrentContext();
				ctx.SaveState();

				bucketIndex = (nuint)Math.Min((int)bucketIndex, this._options.BucketColors.Length - 1);
				var backColor = this._options.BucketColors[bucketIndex];
				ctx.SetFillColor(backColor.ToCGColor());
				ctx.FillEllipseInRect(rect);
				ctx.RestoreState();

				// Draw the text
				UIColor.White.SetColor();
				var textRect = RectangleFExtensions.Inset(rect, (rect.Size.Width - textSize.Width) / 2,
					(rect.Size.Height - textSize.Height) / 2);
				nsText.DrawString(RectangleFExtensions.Integral(textRect), attributes);

				var newImage = UIGraphics.GetImageFromCurrentImageContext();
				UIGraphics.EndImageContext();

				this._iconCache.SetObjectforKey(newImage, nsText);

				return newImage;
			}
		}

		#endregion

		#region IGMUClusterManagerDelegate

		internal class GMUClusterDelegateHandler : NSObject, IMapViewDelegate, IGMUClusterManagerDelegate, IGMUClusterRendererDelegate
		{
			private ClusterLogic _logic;

			public GMUClusterDelegateHandler(ClusterLogic parent)
			{
				this._logic = parent;
			}

			[Export("renderer:willRenderMarker:")]
			public void WillRenderMarker(GMUClusterRenderer renderer, Overlay marker)
			{
				if (marker is Marker) // We may get here markers too...
				{
					var myMarker = (Marker)marker;

					if (myMarker.UserData is ClusteredMarker)
					{
						var item = (ClusteredMarker)myMarker.UserData;
						myMarker.Title = item.Title;
					}
				}
			}
		}

		#endregion
	}
}
