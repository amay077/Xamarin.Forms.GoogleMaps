using Xamarin.Forms;

namespace XFGoogleMapSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            buttonBasicMap.Clicked += (_, e) => Navigation.PushAsync(new BasicMapPage());
            buttonPins.Clicked += (_, e) => Navigation.PushAsync(new PinsPage());
            buttonShapes.Clicked += (_, e) => Navigation.PushAsync(new ShapesPage());
			buttonTiles.Clicked += (_, e) => Navigation.PushAsync(new TilesPage());
            buttonCustomPins.Clicked += (_, e) => Navigation.PushAsync(new CustomPinsPage());
            buttonShapesWithInitialize.Clicked += (_, e) => Navigation.PushAsync(new ShapesWithInitializePage());
			buttonBindingPin.Clicked += (_, e) => Navigation.PushAsync(new BindingPinViewPage());
            buttonGroundOverlays.Clicked += (_, e) => Navigation.PushAsync(new GroundOverlaysPage());
        }
    }
}

