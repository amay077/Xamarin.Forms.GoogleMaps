using Microsoft.Maui.Handlers;

namespace Maui.GoogleMaps.Handlers
{
    public partial class MapHandler
    {
        public static PropertyMapper<Map, MapHandler> MapMapper = new(ViewMapper)
        {
            [nameof(IMap.MapType)] = MapMapType,
            [nameof(IMap.Padding)] = MapPadding,
            [nameof(IMap.IsTrafficEnabled)] = MapIsTrafficEnabled,
            [nameof(IMap.IsIndoorEnabled)] = MapIsIndoorEnabled,
            [nameof(IMap.MyLocationEnabled)] = MapMyLocationEnabled,
            [nameof(IMap.MapStyle)] = MapMapStyle,
            [nameof(IMap.SelectedPin)] = MapSelectedPin,
        };

        public MapHandler() : base(MapMapper)
        {
        }
    }
}
