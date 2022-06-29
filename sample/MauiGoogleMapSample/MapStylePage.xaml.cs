using Maui.GoogleMaps;

namespace MauiGoogleMapSample
{
    public partial class MapStylePage : ContentPage
    {
        public MapStylePage()
        {
            InitializeComponent();

            // Hide all labels and change water fill color
            // You can create your original map style using https://mapstyle.withgoogle.com/ .
            editorMapStyle.Text =
                "[\n" +
                "  {\n" +
                "    \"elementType\": \"labels\",\n" +
                "    \"stylers\": [\n" +
                "      {\n" +
                "        \"visibility\": \"off\"\n" +
                "      }\n" +
                "    ]\n" +
                "  },\n" +
                "  {\n" +
                "    \"featureType\": \"water\",\n" +
                "    \"elementType\": \"geometry.fill\",\n" +
                "    \"stylers\": [\n" +
                "      {\n" +
                "        \"color\": \"#4c4c4c\"\n" +
                "      }\n" +
                "    ]\n" +
                "  }" +
                "]";

            buttonClearStyle.Clicked += (sender, e) =>
            {
                map.MapStyle = null;
            };

            buttonMapStyle.Clicked += (sender, e) => 
            {
                map.MapStyle = MapStyle.FromJson(editorMapStyle.Text);
            };

            buttonSeeStylingWizard.Clicked += (sender, e) => 
            {
                Launcher.OpenAsync(new Uri("https://mapstyle.withgoogle.com/"));
            };

        }
    }
}
