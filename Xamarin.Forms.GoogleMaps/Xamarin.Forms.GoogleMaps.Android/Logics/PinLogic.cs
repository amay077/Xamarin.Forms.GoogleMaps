using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Gms.Maps;
using System.Linq;
using System.ComponentModel;
using Xamarin.Forms.GoogleMaps.Android;
using Xamarin.Forms.GoogleMaps.Android.Extensions;
using NativeBitmapDescriptorFactory = Android.Gms.Maps.Model.BitmapDescriptorFactory;
using Android.Widget;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class PinLogic : DefaultPinLogic<Marker, GoogleMap>
    {
        protected override IList<Pin> GetItems(Map map) => map.Pins;

        private volatile bool _onMarkerEvent = false;
        private Pin _draggingPin;
        private volatile bool _withoutUpdateNative = false;


        internal override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.InfoWindowClick += OnInfoWindowClick;
                newNativeMap.MarkerClick += OnMakerClick;
                newNativeMap.InfoWindowClose += OnInfoWindowClose;
                newNativeMap.MarkerDragStart += OnMarkerDragStart;
                newNativeMap.MarkerDragEnd += OnMarkerDragEnd;
                newNativeMap.MarkerDrag += OnMarkerDrag;
            }
        }

        internal override void Unregister(GoogleMap nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.MarkerDrag -= OnMarkerDrag;
                nativeMap.MarkerDragEnd -= OnMarkerDragEnd;
                nativeMap.MarkerDragStart -= OnMarkerDragStart;
                nativeMap.MarkerClick -= OnMakerClick;
                nativeMap.InfoWindowClose -= OnInfoWindowClose;
                nativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            base.Unregister(nativeMap, map);
        }

        protected override Marker CreateNativeItem(Pin outerItem)
        {
            var opts = new MarkerOptions()
                .SetPosition(new LatLng(outerItem.Position.Latitude, outerItem.Position.Longitude))
                .SetTitle(outerItem.Label)
                .SetSnippet(outerItem.Address)
                .SetSnippet(outerItem.Address)
                .Draggable(outerItem.IsDraggable)
                ;

            if (outerItem.Icon != null)
            {
                opts.SetIcon(outerItem.Icon.ToBitmapDescriptor());
            }

            var marker = NativeMap.AddMarker(opts);
            // If the pin has an IconView set this method will convert it into an icon for the marker
            if (outerItem?.Icon?.Type == BitmapDescriptorType.View)
            {
                marker.Visible = false; // Will become visible once the iconview is ready.
                TransformXamarinViewToAndroidBitmap(outerItem, marker);
            }

            // associate pin with marker for later lookup in event handlers
            outerItem.NativeObject = marker;
            return marker;
        }

        protected override Marker DeleteNativeItem(Pin outerItem)
        {
            var marker = outerItem.NativeObject as Marker;
            if (marker == null)
                return null;
            marker.Remove();
            outerItem.NativeObject = null;

            if (ReferenceEquals(Map.SelectedPin, outerItem))
                Map.SelectedPin = null;

            return marker;
        }

        Pin LookupPin(Marker marker)
        {
            return GetItems(Map).FirstOrDefault(outerItem => ((Marker)outerItem.NativeObject).Id == marker.Id);
        }

        void OnInfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            // lookup pin
            var targetPin = LookupPin(e.Marker);

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetPin?.SendTap();
        }

        void OnMakerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            // lookup pin
            var targetPin = LookupPin(e.Marker);

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

            e.Handled = false;
        }

        void OnInfoWindowClose(object sender, GoogleMap.InfoWindowCloseEventArgs e)
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

        void OnMarkerDragStart(object sender, GoogleMap.MarkerDragStartEventArgs e)
        {
            // lookup pin
            _draggingPin = LookupPin(e.Marker);

            if (_draggingPin != null)
            {
                UpdatePositionWithoutMove(_draggingPin, e.Marker.Position.ToPosition());
                Map.SendPinDragStart(_draggingPin);
            }
        }

        void OnMarkerDrag(object sender, GoogleMap.MarkerDragEventArgs e)
        {
            if (_draggingPin != null)
            {
                UpdatePositionWithoutMove(_draggingPin, e.Marker.Position.ToPosition());
                Map.SendPinDragging(_draggingPin);
            }
        }

        void OnMarkerDragEnd(object sender, GoogleMap.MarkerDragEndEventArgs e)
        {
            if (_draggingPin != null)
            {
                UpdatePositionWithoutMove(_draggingPin, e.Marker.Position.ToPosition());
                Map.SendPinDragEnd(_draggingPin);
                _draggingPin = null;
            }
            _withoutUpdateNative = false;
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
            if (pin == null)
            {
                foreach (var outerItem in GetItems(Map))
                {
                    (outerItem.NativeObject as Marker)?.HideInfoWindow();
                }
            }
            else
            {
                // lookup pin
                var targetPin = LookupPin(pin.NativeObject as Marker);
                (targetPin?.NativeObject as Marker)?.ShowInfoWindow();
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

        protected override void OnUpdateAddress(Pin outerItem, Marker nativeItem)
            => nativeItem.Snippet = outerItem.Address;

        protected override void OnUpdateLabel(Pin outerItem, Marker nativeItem)
            => nativeItem.Title = outerItem.Label;

        protected override void OnUpdatePosition(Pin outerItem, Marker nativeItem)
        {
            if (!_withoutUpdateNative)
            {
                nativeItem.Position = outerItem.Position.ToLatLng();
            }
        }

        protected override void OnUpdateType(Pin outerItem, Marker nativeItem)
        {
        }

        protected override void OnUpdateIcon(Pin outerItem, Marker nativeItem)
        {
            if (outerItem.Icon != null && outerItem.Icon.Type == BitmapDescriptorType.View) 
            {
                // If the pin has an IconView set this method will convert it into an icon for the marker
                TransformXamarinViewToAndroidBitmap(outerItem, nativeItem);       
            }
            else
            {
                nativeItem.SetIcon(outerItem?.Icon?.ToBitmapDescriptor() ?? NativeBitmapDescriptorFactory.DefaultMarker());
                nativeItem.SetAnchor(0.5f, 1f);
                nativeItem.SetInfoWindowAnchor(0.5f, 0f);
            }
        }

        private async void TransformXamarinViewToAndroidBitmap(Pin outerItem, Marker nativeItem)
        {
            if (outerItem?.Icon?.Type == BitmapDescriptorType.View && outerItem?.Icon?.View != null)
            {
                var iconView = outerItem.Icon.View;
                var nativeView = await Utils.ConvertFormsToNative(iconView, new Rectangle(0, 0, (double)Utils.DpToPx((float)iconView.WidthRequest), (double)Utils.DpToPx((float)iconView.HeightRequest)), Platform.Android.Platform.CreateRenderer(iconView));
                var otherView = new FrameLayout(nativeView.Context);
                nativeView.LayoutParameters = new FrameLayout.LayoutParams(Utils.DpToPx((float)iconView.WidthRequest), Utils.DpToPx((float)iconView.HeightRequest));
                otherView.AddView(nativeView);
                nativeItem.SetIcon(await Utils.ConvertViewToBitmapDescriptor(otherView));
                nativeItem.SetAnchor((float)iconView.AnchorX, (float)iconView.AnchorY);
                nativeItem.Visible = true;
            }
        }

        protected override void OnUpdateIsDraggable(Pin outerItem, Marker nativeItem)
        {
            nativeItem.Draggable = outerItem?.IsDraggable ?? false;
        }
    }
}

