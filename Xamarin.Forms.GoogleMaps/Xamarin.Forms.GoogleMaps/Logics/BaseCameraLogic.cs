using Xamarin.Forms.GoogleMaps.Internals;

namespace Xamarin.Forms.GoogleMaps.Logics
{
    internal abstract class BaseCameraLogic<TNativeMap> : IMapRequestDelegate
    {
        protected Map _map;
        protected TNativeMap _nativeMap;

        public float ScaledDensity { get; internal set; }

        public void Register(Map map, TNativeMap nativeMap)
        {
            _map = map;
            _nativeMap = nativeMap;

            _map.OnMoveToRegion = OnMoveToRegionRequest;
            _map.OnMoveCamera = OnMoveCameraRequest;
        }

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