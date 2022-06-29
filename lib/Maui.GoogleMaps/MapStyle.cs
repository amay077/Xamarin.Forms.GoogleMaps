
namespace Maui.GoogleMaps
{
    public sealed class MapStyle
    {
        public static MapStyle FromJson(string jsonStyle)
        {
            return new MapStyle(jsonStyle);
        }

        public string JsonStyle { get; }

        private MapStyle(string jsonStyle)
        {
            this.JsonStyle = jsonStyle;
        }
    }
}
