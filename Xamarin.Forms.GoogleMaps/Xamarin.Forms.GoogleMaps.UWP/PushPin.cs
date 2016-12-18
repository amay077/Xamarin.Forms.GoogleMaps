using System;
using System.ComponentModel;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using Xamarin.Forms.GoogleMaps.Extensions.UWP;
using Xamarin.Forms.Platform.UWP;

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

        public StackPanel Root { get; set; } = new StackPanel() { Width = 250 };
        public StackPanel DetailsView { get; set; }
        public TextBlock PinLabel { get; set; }
        public TextBlock Address { get; set; }
        public FrameworkElement Icon { get; set; }

        internal PushPin(Pin pin)
        {
            if (pin == null)
                throw new ArgumentNullException();

            SetupDetailsView(pin);
            UpdateIcon(pin);

            Content = Root;

            Id = Guid.NewGuid();
            DataContext = _pin = pin;

            UpdateLocation();

            pin.NativeObject = this;
        }

        private void SetupDetailsView(Pin pin)
        {
            //Setup details view
            DetailsView = new StackPanel()
            {
                Width = 250,
                Height = 70,
                Opacity = 0.7,
                Padding = new Windows.UI.Xaml.Thickness(5),
                Background = new SolidColorBrush(Colors.White)
            };
            PinLabel = new TextBlock()
            {
                Text = pin.Label,
                Foreground = new SolidColorBrush(Colors.Black),
                FontWeight = FontWeights.Bold,
                TextWrapping = TextWrapping.WrapWholeWords,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            if (!string.IsNullOrEmpty(pin.Address))
            {
                Address = new TextBlock()
                {
                    Text = pin.Address,
                    Foreground = new SolidColorBrush(Colors.Black),
                    HorizontalAlignment = HorizontalAlignment.Center
                };
            }
            DetailsView.Children.Add(PinLabel);
            if (!string.IsNullOrEmpty(pin.Address))
            {
                DetailsView.Children.Add(Address);
            }
            else
            {
                DetailsView.Height = 35;
            }
            DetailsView.Visibility = Visibility.Collapsed;
            Root.Children.Add(DetailsView);
        }

        public void UpdateIcon(Pin pin)
        {
            if (pin.Icon == null || pin.Icon.Type == BitmapDescriptorType.Default)
            {
                var template = Windows.UI.Xaml.Application.Current.Resources["PushPinTemplate"] as Windows.UI.Xaml.DataTemplate;
                var content = template.LoadContent();
                var path = content as Path;
                if (path != null)
                {
                    if (pin.Icon != null && pin.Icon.Color != Color.Black)
                    {
                        var converter = new ColorConverter();
                        var colour =
                        path.Fill = (SolidColorBrush)converter.Convert(pin.Icon.Color, null, null, null); ;
                    }
                    if (Icon != null)
                    {
                        Root.Children.Remove(Icon);
                    }
                    Icon = path;
                    Root.Children.Add(Icon);
                }
            }
            else
            {
                if (pin.Icon.Type != BitmapDescriptorType.View)
                {
                    var image = new Windows.UI.Xaml.Controls.Image()
                    {
                        Source = pin.Icon.ToBitmapDescriptor(),
                        Width = 50,
                    };
                    if (Icon != null)
                    {
                        Root.Children.Remove(Icon);
                    }
                    Icon = image;
                    Root.Children.Add(Icon);
                }
                else
                {
                    TransformXamarinViewToUWPBitmap(pin, this);
                }
            }
        }

        public void UpdateLocation()
        {
            var anchor = new Windows.Foundation.Point(0.5, 1);
            var location = new Geopoint(new BasicGeoposition
            {
                Latitude = _pin.Position.Latitude,
                Longitude = _pin.Position.Longitude
            });
            MapControl.SetLocation(this, location);
            MapControl.SetNormalizedAnchorPoint(this, anchor);
        }

        //TODO: implement xamarin view to UWP
        private async void TransformXamarinViewToUWPBitmap(Pin outerItem, ContentControl nativeItem)
        {
            if (outerItem?.Icon?.Type == BitmapDescriptorType.View && outerItem?.Icon?.View != null)
            {
                //var iconView = outerItem.Icon.View;
                //var nativeView = await Utils.ConvertFormsToNative(iconView, new Rectangle(0, 0, (double)Utils.DpToPx((float)iconView.WidthRequest), (double)Utils.DpToPx((float)iconView.HeightRequest)), Platform.Android.Platform.CreateRenderer(iconView));
                //var otherView = new FrameLayout(nativeView.Context);
                //nativeView.LayoutParameters = new FrameLayout.LayoutParams(Utils.DpToPx((float)iconView.WidthRequest), Utils.DpToPx((float)iconView.HeightRequest));
                //otherView.AddView(nativeView);
                //nativeItem.SetIcon(await Utils.ConvertViewToBitmapDescriptor(otherView));
                //nativeItem.SetAnchor((float)iconView.AnchorX, (float)iconView.AnchorY);
                //nativeItem.Visible = true;
            }
        }
    }
}