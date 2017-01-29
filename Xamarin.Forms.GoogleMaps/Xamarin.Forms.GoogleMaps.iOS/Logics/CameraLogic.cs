using Google.Maps;
using Xamarin.Forms.GoogleMaps.Internals;

namespace Xamarin.Forms.GoogleMaps.Logics.iOS
{
    internal sealed class CameraLogic : BaseCameraLogic<MapView>
    {
        public override void Register(Map map, MapView nativeMap)
        {
            _map = map;
            _nativeMap = nativeMap;
        }

        public override void OnMoveToRegionRequest(MoveToRegionMessage m)
        {
        }

        public override void OnMoveCameraRequest(CameraUpdateMessage m)
        {
        }
    }
}