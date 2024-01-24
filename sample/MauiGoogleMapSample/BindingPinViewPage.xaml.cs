using Maui.GoogleMaps;

namespace MauiGoogleMapSample
{
    public partial class BindingPinViewPage : ContentPage
    {
        public BindingPinViewPage()
        {
            InitializeComponent();

            map.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(new Position(35.71d, 139.81d), 12d);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await Task.Delay(1000); // workaround for #30 [Android]Map.Pins.Add doesn't work when page OnAppearing

            var pin = new Pin()
            {
                Type = PinType.Place,
                Label = "Tokyo SKYTREE",
                Address = "Sumida-ku, Tokyo, Japan",
                Position = new Position(35.71d, 139.81d),
                Icon = BitmapDescriptorFactory.FromView(() => new BindingPinView(pinDisplay.Text))
            };
            map.Pins.Add(pin);
            pinDisplay.TextChanged += (sender, e) =>
            {
                pin.Icon = BitmapDescriptorFactory.FromView(() => new BindingPinView(e.NewTextValue));
            };
        }
    }
}

