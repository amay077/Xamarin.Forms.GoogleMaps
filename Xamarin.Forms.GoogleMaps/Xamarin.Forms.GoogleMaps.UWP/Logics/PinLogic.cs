using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Xamarin.Forms.GoogleMaps.Extensions.UWP;
using Xamarin.Forms.GoogleMaps.UWP;

namespace Xamarin.Forms.GoogleMaps.Logics.UWP
{
    internal class PinLogic : DefaultPinLogic<PushPin, MapControl>
    {
        internal override void Register(MapControl oldNativeMap, Map oldMap, MapControl newNativeMap, Map newMap)
        {
            base.Register(oldNativeMap, oldMap, newNativeMap, newMap);
            if (newNativeMap != null)
            {
                newNativeMap.MapTapped += NewNativeMap_MapTapped;
                newNativeMap.MapHolding += NewNativeMap_MapHolding;
            }
        }



        internal override void Unregister(MapControl nativeMap, Map map)
        {
            if (nativeMap != null)
            {
                nativeMap.MapTapped -= NewNativeMap_MapTapped;
                nativeMap.MapHolding -= NewNativeMap_MapHolding;

                foreach (var pin in nativeMap.Children.OfType<PushPin>())
                {
                    pin.Tapped -= Pushpin_Tapped;
                    pin.Holding -= Pushpin_Holding;
                    pin.InfoWindowClicked -= PushpinOnInfoWindowClicked;
                }
            }
        }

        private void NewNativeMap_MapTapped(MapControl sender, MapInputEventArgs args)
        {
            Map.SendMapClicked(args.Location.Position.ToPosition());
            //UnselectPin
            var pin = Map.SelectedPin;
            if (pin != null)
            {
                foreach (var outerItem in GetItems(Map))
                {
                    if ((outerItem.NativeObject as PushPin).DetailsView.Visibility == Windows.UI.Xaml.Visibility.Visible)
                    {
                        (outerItem.NativeObject as PushPin).DetailsView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                }
            }
            if (Map.SelectedPin != null)
            {
                Map.SelectedPin = null;
                Map.SendSelectedPinChanged(null);
            }
        }

        private void NewNativeMap_MapHolding(MapControl sender, MapInputEventArgs args)
        {
            Map.SendMapLongClicked(args.Location.Position.ToPosition());
        }

        protected override PushPin CreateNativeItem(Pin outerItem)
        {
            var pushpin = new PushPin(outerItem);
            pushpin.Tapped += Pushpin_Tapped;
            pushpin.Holding += Pushpin_Holding;
            pushpin.InfoWindowClicked += PushpinOnInfoWindowClicked;

            pushpin.Visibility = outerItem?.IsVisible ?? false ?
                Windows.UI.Xaml.Visibility.Visible :
                Windows.UI.Xaml.Visibility.Collapsed;

            NativeMap.Children.Add(pushpin);
            return pushpin;
        }

        private void PushpinOnInfoWindowClicked(object sender, TappedRoutedEventArgs tappedRoutedEventArgs)
        {
            Map.SendInfoWindowClicked(((Pin)sender));
        }

        private void Pushpin_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
        {
            //drag not implemented
        }

        private void Pushpin_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            //Select pin
            var pin = (sender as PushPin);
            if (Map.SelectedPin != null)
            {
                foreach (var outerItem in GetItems(Map))
                {
                    if ((outerItem.NativeObject as PushPin).DetailsView.Visibility == Windows.UI.Xaml.Visibility.Visible)
                    {
                        (outerItem.NativeObject as PushPin).DetailsView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }
                }
            }
            pin.DetailsView.Visibility = pin.DetailsView.Visibility == Windows.UI.Xaml.Visibility.Visible ? Windows.UI.Xaml.Visibility.Collapsed : Windows.UI.Xaml.Visibility.Visible;
            var targetPin = LookupPin(pin);
            // If set to PinClickedEventArgs.Handled = true in app codes,
            // then all pin selection controlling by app.
            if (Map.SendPinClicked(targetPin))
            {
                return;
            }

            if (targetPin != null && !ReferenceEquals(targetPin, Map.SelectedPin))
            {
                if (Map.SelectedPin != null)
                {
                    Map.SendSelectedPinChanged(null);
                }
                Map.SelectedPin = targetPin;
            }
            else
            {
                Map.SelectedPin = null;
            }
            Map.SendSelectedPinChanged(Map.SelectedPin);
        }

        Pin LookupPin(PushPin marker)
        {
            return Map.Pins.FirstOrDefault(outerItem => ((PushPin)outerItem.NativeObject).Id == marker.Id);
        }

        protected override PushPin DeleteNativeItem(Pin outerItem)
        {
            var nativePushpin = outerItem.NativeObject as PushPin;

            if (nativePushpin == null)
            {
                return null;
            }

            nativePushpin.Tapped -= Pushpin_Tapped;
            nativePushpin.Holding -= Pushpin_Holding;
            nativePushpin.InfoWindowClicked -= PushpinOnInfoWindowClicked;

            NativeMap.Children.Remove(nativePushpin);

            outerItem.NativeObject = null;

            if (ReferenceEquals(Map.SelectedPin, outerItem))
                Map.SelectedPin = null;

            return nativePushpin;
        }

        protected override IList<Pin> GetItems(Map map)
        {
            return map.Pins;
        }

        protected override void OnUpdateAddress(Pin outerItem, PushPin nativeItem)
        {
            nativeItem.Address.Text = outerItem.Address;
        }

        protected override void OnUpdateLabel(Pin outerItem, PushPin nativeItem)
        {
            nativeItem.PinLabel.Text = outerItem.Label;
        }

        protected override void OnUpdateIcon(Pin outerItem, PushPin nativeItem)
        {
            nativeItem.UpdateIcon(outerItem);
        }

        protected override void OnUpdateIsDraggable(Pin outerItem, PushPin nativeItem)
        {
            //Not implemented
        }

        protected override void OnUpdatePosition(Pin outerItem, PushPin nativeItem)
        {
            nativeItem.UpdateLocation();
        }

        protected override void OnUpdateRotation(Pin outerItem, PushPin nativeItem)
        {
            //Not Implemented
        }

        protected override void OnUpdateType(Pin outerItem, PushPin nativeItem)
        {
            //not implemented
        }

        protected override void OnUpdateIsVisible(Pin outerItem, PushPin nativeItem)
        {
            nativeItem.Visibility = outerItem?.IsVisible ?? false ?
                Windows.UI.Xaml.Visibility.Visible :
                Windows.UI.Xaml.Visibility.Collapsed;
        }

        protected override void OnUpdateAnchor(Pin outerItem, PushPin nativeItem)
        {
            //not implemented
        }

        protected override void OnUpdateFlat(Pin outerItem, PushPin nativeItem)
        {
            //not implemented
        }

        protected override void OnUpdateInfoWindowAnchor(Pin outerItem, PushPin nativeItem)
        {
            //not implemented
        }
        protected override void OnUpdateZIndex(Pin outerItem, PushPin nativeItem)
        {
            //not implemented
        }
        protected override void OnUpdateTransparency(Pin outerItem, PushPin nativeItem)
        {
            //not implemented
        }
    }
}
