using Xamarin.Forms.GoogleMaps.Internals;

namespace Xamarin.Forms.GoogleMaps.Logics
{
    internal abstract class BaseCameraLogic<TNativeMap> : IMapRequestDelegate
    {
        public abstract void Register(Map map, TNativeMap nativeMap);
        public abstract void Unregister();

        public abstract void OnMoveToRegionRequest(MoveToRegionMessage m);
        public abstract void OnMoveCameraRequest(CameraUpdateMessage m);
    }
}