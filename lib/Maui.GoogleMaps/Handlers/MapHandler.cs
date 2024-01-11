using Microsoft.Maui.Handlers;
#if ANDROID
using MapView = Android.Gms.Maps.MapView;
#elif IOS
using MapView = Google.Maps.MapView;
#elif (NET6_0_OR_GREATER && !IOS && !ANDROID)
using MapView = System.Object;
#endif

namespace Maui.GoogleMaps.Handlers
{
    public partial class MapHandler : ViewHandler<Map, MapView>
    {
        public static PropertyMapper<Map, MapHandler> MapMapper = new(ViewMapper)
        {
#if ANDROID || IOS
            [nameof(IMap.MapType)] = MapMapType,
            [nameof(IMap.Padding)] = MapPadding,
            [nameof(IMap.IsTrafficEnabled)] = MapIsTrafficEnabled,
            [nameof(IMap.IsIndoorEnabled)] = MapIsIndoorEnabled,
            [nameof(IMap.MyLocationEnabled)] = MapMyLocationEnabled,
            [nameof(IMap.MapStyle)] = MapMapStyle,
            [nameof(IMap.SelectedPin)] = MapSelectedPin,
#endif
        };

        public MapHandler() : base(MapMapper)
        {
        }
#if (NET6_0_OR_GREATER && !IOS && !ANDROID)
        protected override object CreatePlatformView()
        {
            throw new NotImplementedException();
        }
#endif
    }
}
