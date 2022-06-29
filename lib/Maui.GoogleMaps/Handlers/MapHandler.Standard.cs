using Microsoft.Maui.Handlers;

namespace Maui.GoogleMaps.Handlers
{
    public partial class MapHandler : ViewHandler<Map, object>
    {
        protected override object CreatePlatformView() => throw new NotImplementedException();
        public static void MapMapType(MapHandler handler, Map map) { }
        public static void MapPadding(MapHandler handler, Map map) { }
        public static void MapIsTrafficEnabled(MapHandler handler, Map map) { }
        public static void MapIsIndoorEnabled(MapHandler handler, Map map) { }
        public static void MapMyLocationEnabled(MapHandler handler, Map map) { }
        public static void MapMapStyle(MapHandler handler, Map map) { }
        public static void MapSelectedPin(MapHandler handler, Map map) { }
    }
}
