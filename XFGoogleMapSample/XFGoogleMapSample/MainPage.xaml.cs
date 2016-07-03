using Xamarin.Forms;
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
    }
}

