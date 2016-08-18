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

