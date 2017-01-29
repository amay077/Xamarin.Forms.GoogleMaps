using Xamarin.Forms.GoogleMaps.Internals;

namespace Xamarin.Forms.GoogleMaps.Logics
{
    internal abstract class BaseCameraLogic<TNativeMap> : IMapRequestDelegate
    {
        protected Map _map;
        protected TNativeMap _nativeMap;

        public abstract void Register(Map map, TNativeMap nativeMap);

        public void Unregister()
        {
            if (_map != null)
            {
                _map.OnMoveToRegion = null;
                _map.OnMoveCamera = null;
            }
        }

        public abstract void OnMoveToRegionRequest(MoveToRegionMessage m);
        public abstract void OnMoveCameraRequest(CameraUpdateMessage m);
    }
}