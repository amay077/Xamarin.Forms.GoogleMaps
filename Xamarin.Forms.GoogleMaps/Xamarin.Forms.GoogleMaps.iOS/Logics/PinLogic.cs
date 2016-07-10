using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Google.Maps;
using Xamarin.Forms.GoogleMaps.Extensions.iOS;
namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
    internal class PinLogic : DefaultPinLogic<Marker, MapView>
    {
        protected override IList<Pin> GetItems(Map map) => map.Pins;

        bool _onMarkerEvent;

        internal override void Register(MapView oldNativeMap, Map oldMap, MapView newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.InfoTapped += OnInfoTapped;
                newNativeMap.TappedMarker = HandleGMSTappedMarker;
                newNativeMap.InfoClosed += InfoWindowClosed;
            }

        }

        internal override void Unregister(MapView nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.InfoClosed -= InfoWindowClosed;
                nativeMap.TappedMarker = null;
                nativeMap.InfoTapped -= OnInfoTapped;
            }

            base.Unregister(nativeMap, map);
        }

        protected override Marker CreateNativeItem(Pin outerItem)
        {
            var nativeMarker = Marker.FromPosition(outerItem.Position.ToCoord());
            nativeMarker.Title = outerItem.Label;
            nativeMarker.Snippet = outerItem.Address ?? string.Empty;
            outerItem.NativeObject = nativeMarker;
            nativeMarker.Map = NativeMap;
            return nativeMarker;
        }

        protected override Marker DeleteNativeItem(Pin outerItem)
        {
            var nativeMarker = outerItem.NativeObject as Marker;
            nativeMarker.Map = null;

            if (object.ReferenceEquals(Map.SelectedPin, outerItem))
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
                NativeMap.SelectedMarker = (Marker)pin.NativeObject;
            else
                NativeMap.SelectedMarker = null;
        }

        Pin LookupPin(Marker marker)
        {
            return GetItems(Map).FirstOrDefault(outerItem => object.ReferenceEquals(outerItem.NativeObject, marker));
        }

        void OnInfoTapped(object sender, GMSMarkerEventEventArgs e)
        {
            // lookup pin
            var targetPin = LookupPin(e.Marker);

            // only consider event handled if a handler is present. 
            // Else allow default behavior of displaying an info window.
            targetPin?.SendTap();
        }

        bool HandleGMSTappedMarker(MapView mapView, Marker marker)
        {
            // lookup pin
            var targetPin = LookupPin(marker);

            try
            {
                _onMarkerEvent = true;
                if (targetPin != null && !object.ReferenceEquals(targetPin, Map.SelectedPin))
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
                if (targetPin != null && object.ReferenceEquals(targetPin, Map.SelectedPin))
                    Map.SelectedPin = null;
            }
            finally
            {
                _onMarkerEvent = false;
            }
        }

        protected override void OnUpdateAddress(Pin outerItem, Marker nativeItem)
            => nativeItem.Snippet = outerItem.Address;

        protected override void OnUpdateLabel(Pin outerItem, Marker nativeItem)
            => nativeItem.Title = outerItem.Label;

        protected override void OnUpdatePosition(Pin outerItem, Marker nativeItem)
            => nativeItem.Position = outerItem.Position.ToCoord();

        protected override void OnUpdateType(Pin outerItem, Marker nativeItem)
        {
        }
    }
}

