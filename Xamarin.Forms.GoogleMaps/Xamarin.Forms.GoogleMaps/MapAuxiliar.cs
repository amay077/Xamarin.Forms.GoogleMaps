// Original code from https://github.com/javiholcman/Wapps.Forms.Map/
// This class needs to be instantiated in the page that renders the map.
// Only really needed for Android.

namespace Xamarin.Forms.GoogleMaps
{
    // this layout is used to render the android native views
    public class MapAuxiliar : StackLayout
    {
        public static MapAuxiliar LiveMap { get; set; }

        public MapAuxiliar()
        {
            LiveMap = this;
            this.Opacity = 0;
            this.WidthRequest = 1;
            this.HeightRequest = 1;
        }
    }
}

