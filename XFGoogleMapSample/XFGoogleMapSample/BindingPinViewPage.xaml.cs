using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;

namespace XFGoogleMapSample
{
	public partial class BindingPinViewPage : ContentPage
	{
		public BindingPinViewPage()
		{
			InitializeComponent();
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
                Icon = BitmapDescriptorFactory.FromView(new BindingPinView(pinDisplay.Text))
			};
			map.Pins.Add(pin);
			map.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMeters(5000)));
			pinDisplay.TextChanged += (sender, e) =>
			{
                pin.Icon = BitmapDescriptorFactory.FromView(new BindingPinView(e.NewTextValue));
			};
		}
	}
}

