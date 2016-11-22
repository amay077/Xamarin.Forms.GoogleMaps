using System;
using System.ComponentModel;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Xamarin.Forms.GoogleMaps.Extensions.UWP;

#if WINDOWS_UWP

namespace Xamarin.Forms.GoogleMaps.UWP
#else

namespace Xamarin.Forms.Maps.WinRT
#endif
{
    internal class PushPin : ContentControl
    {
        readonly Pin _pin;

        public Guid Id { get; set; }

        internal PushPin(Pin pin)
        {
            if (pin == null)
                throw new ArgumentNullException();

            if (pin.Icon == null)
            {
                ContentTemplate = Windows.UI.Xaml.Application.Current.Resources["PushPinTemplate"] as Windows.UI.Xaml.DataTemplate;
            }
            else
            {
                //overwriting content later?
                Content = pin.Icon.ToBitmapDescriptor();
            }

            Id = Guid.NewGuid();
            DataContext = Content = _pin = pin;

            UpdateLocation();

            Loaded += PushPinLoaded;
            Unloaded += PushPinUnloaded;
            //Tapped += PushPinTapped;
            pin.NativeObject = this;
        }

        void PushPinLoaded(object sender, RoutedEventArgs e)
        {
            _pin.PropertyChanged += PinPropertyChanged;
        }

        void PushPinUnloaded(object sender, RoutedEventArgs e)
        {
            _pin.PropertyChanged -= PinPropertyChanged;
           // Tapped -= PushPinTapped;
        }

        void PinPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Pin.PositionProperty.PropertyName)
                UpdateLocation();
        }

        //void PushPinTapped(object sender, TappedRoutedEventArgs e)
        //{
        //    _pin.SendTap();
        //}

        void UpdateLocation()
        {
            var anchor = new Windows.Foundation.Point(0.65, 1);
            var location = new Geopoint(new BasicGeoposition
            {
                Latitude = _pin.Position.Latitude,
                Longitude = _pin.Position.Longitude
            });
            MapControl.SetLocation(this, location);
            MapControl.SetNormalizedAnchorPoint(this, anchor);
        }
    }
}