
namespace Maui.GoogleMaps.Internals;

internal interface IMapRequestDelegate
{
    void OnMoveToRegionRequest(MoveToRegionMessage m);
    void OnMoveCameraRequest(CameraUpdateMessage m);
}
