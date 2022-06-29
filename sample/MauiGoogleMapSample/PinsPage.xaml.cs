using Maui.GoogleMaps;

namespace MauiGoogleMapSample
{
    public partial class PinsPage : ContentPage
    {
        public PinsPage()
        {
            InitializeComponent();

            Pin pinTokyo = null;
            Pin pinNewYork = null;

            // Tokyo pin
            buttonAddPinTokyo.Clicked += (sender, e) =>
            {
                pinTokyo = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Tokyo SKYTREE",
                    Address = "Sumida-ku, Tokyo, Japan",
                    Position = new Position(35.71d, 139.81d),
                    Rotation = 33.3f,
                    Tag = "id_tokyo",
                    IsVisible = switchIsVisibleTokyo.IsToggled
                };

                map.Pins.Add(pinTokyo);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(pinTokyo.Position, Distance.FromMeters(5000)));

                ((Button)sender).IsEnabled = false;
                buttonRemovePinTokyo.IsEnabled = true;
            };

            buttonRemovePinTokyo.Clicked += (sender, e) =>
            {
                map.Pins.Remove(pinTokyo);
                pinTokyo = null;
                ((Button)sender).IsEnabled = false;
                buttonAddPinTokyo.IsEnabled = true;
            };
            buttonRemovePinTokyo.IsEnabled = false;

            // New York pin
            buttonAddPinNewYork.Clicked += (sender, e) =>
            {
                pinNewYork = new Pin()
                {
                    Type = PinType.Place,
                    Label = "Central Park NYC",
                    Address = "New York City, NY 10022",
                    Position = new Position(40.78d, -73.96d),
                    Tag = "id_new_york"
                };

                map.Pins.Add(pinNewYork);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(pinNewYork.Position, Distance.FromMeters(5000)));

                ((Button)sender).IsEnabled = false;
                buttonRemovePinNewYork.IsEnabled = true;
            };

            buttonRemovePinNewYork.Clicked += (sender, e) =>
            {
                map.Pins.Remove(pinNewYork);
                pinNewYork = null;
                ((Button)sender).IsEnabled = false;
                buttonAddPinNewYork.IsEnabled = true;
            };
            buttonRemovePinNewYork.IsEnabled = false;

            // Clear Pins
            buttonClearPins.Clicked += (sender, e) =>
            {
                map.Pins.Clear();

                pinTokyo = null;
                pinNewYork = null;
                buttonAddPinTokyo.IsEnabled = true;
                buttonAddPinNewYork.IsEnabled = true;
                buttonRemovePinTokyo.IsEnabled = false;
                buttonRemovePinNewYork.IsEnabled = false;
            };

            // Select New York Pin
            buttonSelectPinNewYork.Clicked += (sender, e) =>
            {
                if (pinNewYork == null)
                {
                    DisplayAlert("Error", "New York is not added.", "Close");
                    return;
                }

                map.SelectedPin = pinNewYork;
            };

            // Clear Pin Selection
            buttonClearSelection.Clicked += (sender, e) =>
            {
                if (map.SelectedPin == null)
                {
                    DisplayAlert("Error", "Pin is not selected.", "Close");
                    return;
                }

                map.SelectedPin = null;
            };


            // Visible/Invisible Pin tokyo
            switchIsVisibleTokyo.Toggled += (sender, args) =>
            {
                if (pinTokyo != null)
                {
                     pinTokyo.IsVisible = args.Value;
                }
            };

            // AnchorX Pin new york
            sliderAnchorXNewyork.ValueChanged += (sender, args) =>
            {
                pinNewYork.Anchor = new Point(args.NewValue / 100d, pinNewYork.Anchor.Y);
            };

            // AnchorY Pin new york
            sliderAnchorYNewyork.ValueChanged += (sender, args) =>
            {
                pinNewYork.Anchor = new Point(pinNewYork.Anchor.X, args.NewValue / 100d);
            };

            map.PinClicked += Map_PinClicked;;

            // Selected Pin changed
            map.SelectedPinChanged += SelectedPin_Changed;

            map.InfoWindowClicked += InfoWindow_Clicked;

            map.InfoWindowLongClicked += InfoWindow_LongClicked;
        }

        private void InfoWindow_LongClicked(object sender, InfoWindowLongClickedEventArgs e)
        {
           
            var time = DateTime.Now.ToString("hh:mm:ss");
            labelStatus.Text = $"[{time}]InfoWindow Long Clicked - {e?.Pin?.Tag.ToString() ?? "nothing"}";
        }

        private void InfoWindow_Clicked(object sender, InfoWindowClickedEventArgs e)
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            labelStatus.Text = $"[{time}]InfoWindow Clicked - {e?.Pin?.Tag.ToString() ?? "nothing"}";
        }

        void SelectedPin_Changed(object sender, SelectedPinChangedEventArgs e)
        {
            var time = DateTime.Now.ToString("hh:mm:ss");
            labelStatus.Text = $"[{time}]SelectedPin changed - {e?.SelectedPin?.Label ?? "nothing"}";
        }

        // Do NOT mark async method.
        // Because Xamarin.Forms.GoogleMaps wait synchronously for this callback returns.
        void Map_PinClicked(object sender, PinClickedEventArgs e)
        {
            e.Handled = switchHandlePinClicked.IsToggled;

            // If you set e.Handled = true,
            // then Pin selection doesn't work automatically.
            // All pin selection operations are delegated to you.
            // Sample codes are below.
            //if (switchHandlePinClicked.IsToggled)
            //{
            //    map.SelectedPin = e.Pin;
            //    map.AnimateCamera(CameraUpdateFactory.NewPosition(e.Pin.Position));
            //}
        }
    }
}

