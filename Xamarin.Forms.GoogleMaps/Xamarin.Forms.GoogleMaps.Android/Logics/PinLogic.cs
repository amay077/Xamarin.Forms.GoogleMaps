﻿using System;
using Android.Gms.Maps.Model;
using System.Collections.Generic;
using Android.Gms.Maps;
using System.Linq;
using System.ComponentModel;
using Xamarin.Forms.GoogleMaps.Android;

namespace Xamarin.Forms.GoogleMaps.Logics.Android
{
    internal class PinLogic : DefaultPinLogic<Marker, GoogleMap>
    {
        protected override IList<Pin> GetItems(Map map) => map.Pins;

        private volatile bool _onMarkerEvent = false;

        internal override void Register(GoogleMap oldNativeMap, Map oldMap, GoogleMap newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);

            if (newNativeMap != null)
            {
                newNativeMap.InfoWindowClick += OnInfoWindowClick;
                newNativeMap.MarkerClick += OnMakerClick;
                newNativeMap.InfoWindowClose += OnInfoWindowClose;
            }
        }

        internal override void Unregister(GoogleMap nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.MarkerClick -= OnMakerClick;
                nativeMap.InfoWindowClose -= OnInfoWindowClose;
                nativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            base.Unregister(nativeMap, map);
        }

        protected override Marker CreateNativeItem(Pin outerItem)
        {
            var opts = new MarkerOptions();
            opts.SetPosition(new LatLng(outerItem.Position.Latitude, outerItem.Position.Longitude));
            opts.SetTitle(outerItem.Label);
            opts.SetSnippet(outerItem.Address);
            var marker = NativeMap.AddMarker(opts);

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

            if (object.ReferenceEquals(Map.SelectedPin, outerItem))
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
                if (targetPin != null && !object.ReferenceEquals(targetPin, Map.SelectedPin))
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
                if (targetPin != null && object.ReferenceEquals(targetPin, Map.SelectedPin))
                    Map.SelectedPin = null;
            }
            finally
            {
                _onMarkerEvent = false;
            }
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

        protected override void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnItemPropertyChanged(sender, e);

            var pin = sender as Pin;
            var marker = pin?.NativeObject as Marker;
            if (marker == null)
                return;

            if (e.PropertyName == Pin.LabelProperty.PropertyName)
            {
                marker.Title = pin.Label;
            }
            else if (e.PropertyName == Pin.AddressProperty.PropertyName)
            {
                marker.Snippet = pin.Address;
            }
            else if (e.PropertyName == Pin.TypeProperty.PropertyName)
            {
                /* no-op */
            }
            else if (e.PropertyName == Pin.PositionProperty.PropertyName)
            {
                marker.Position = pin.Position.ToLatLng();
            }
        }

        protected override void OnUpdateAddress(Pin outerItem, Marker nativeItem)
            => nativeItem.Snippet = outerItem.Address;

        protected override void OnUpdateLabel(Pin outerItem, Marker nativeItem)
            => nativeItem.Title = outerItem.Label;

        protected override void OnUpdatePosition(Pin outerItem, Marker nativeItem)
            => nativeItem.Position = outerItem.Position.ToLatLng();

        protected override void OnUpdateType(Pin outerItem, Marker nativeItem)
        {
        }
    }
}

