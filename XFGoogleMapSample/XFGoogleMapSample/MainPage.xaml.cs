using Xamarin.Forms;
using Xamarin.Forms.GoogleMaps;
using System.Linq;
using System.Threading.Tasks;

namespace XFGoogleMapSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            buttonBasicMap.Clicked += (sender, e) => Navigation.PushAsync(new BasicMapPage());
            buttonPins.Clicked += (sender, e) => Navigation.PushAsync(new PinsPage());
            buttonShapes.Clicked += (sender, e) => Navigation.PushAsync(new ShapesPage());
			buttonTiles.Clicked += (sender, e) => Navigation.PushAsync(new TilesPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            //await Task.Delay(3000);

            //// MapType
            //map.MapType = MapType.Hybrid;

            //// UISettings
            ////map.HasZoomEnabled = false;
            //map.HasScrollEnabled = true;
            //map.IsShowingUser = false;

            //// Add pin
            //var pin = new Pin()
            //{
            //    Type = PinType.Place,
            //    Label = "Tokyo SKYTREE",
            //    Address = "Sumida-ku, Tokyo, Japan",
            //    Position = new Position(35.71d, 139.81d)
            //};
            ////map.Pins.Add(pin);
            ////map.SelectedPin = pin;

            ////// Pin tap event
            ////pin.Clicked += (sender, args) =>
            ////{
            ////    DisplayAlert("tapped", "InfoWindow tapped!", "close");
            ////};

            //// Move
            //map.MoveToRegion(MapSpan.FromCenterAndRadius(pin.Position, Distance.FromMeters(1000)));

            ////await Task.Delay(3000);

            ////var pin2 = new Pin()
            ////{
            ////    Type = PinType.Place,
            ////    Label = "OSHIGAMI",
            ////    Address = "DESC",
            ////    Position = new Position(35.709200, 139.813613)
            ////};
            ////map.Pins.Add(pin2);
            //////pin2.ShowInfoWindow();

            ////await Task.Delay(3000);

            ////map.SelectedPin = pin2;

            ////await Task.Delay(3000);

            ////map.SelectedPin = null;

            ////int a = 1;

            ////pin2.HideInfoWindow();

            //var lines = map.Polylines;

            //var line1 = new Polyline();
            //line1.Positions.Add(new Position(35.708181, 139.804746));
            //line1.Positions.Add(new Position(35.708434, 139.808705));
            //line1.Positions.Add(new Position(35.711788, 139.810003));
            //line1.StrokeColor = Color.Lime;
            //line1.StrokeWidth = 10f;
            //line1.IsClickable = true;

            //line1.Clicked += (s, e) =>
            //{
            //    DisplayAlert("Event", "Polyline Clicked", "close");
            //};

            //lines.Add(line1);
        }
    }
}

