using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using System.Linq;

namespace XFGoogleMapSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // MapType
            map.MapType = MapType.Hybrid;

            // UISettings
            map.HasZoomEnabled = false;
            map.HasScrollEnabled = true;
            map.IsShowingUser = false;

            // Add pin
            var pin = new Pin()
            {
                Type = PinType.Place,
                Label = "Tokyo SKYTREE",
                Address = "Sumida-ku, Tokyo, Japan",
                Position = new Position(35.71d, 139.81d)
            };
            map.Pins.Add(pin);

            // Pin tap event
            pin.Clicked += (sender, args) =>
            {
                DisplayAlert("tapped", "InfoWindow tapped!", "close");
            };

            // Move
            map.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMeters(1000)));
        }
    }
}

