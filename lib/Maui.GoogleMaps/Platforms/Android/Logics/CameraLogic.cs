using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Java.Lang;
using Maui.GoogleMaps.Android.Callbacks;
using Maui.GoogleMaps.Android.Extensions;
using Maui.GoogleMaps.Internals;
using static Android.Gms.Maps.GoogleMap;
using GCameraUpdateFactory = Android.Gms.Maps.CameraUpdateFactory;

namespace Maui.GoogleMaps.Logics.Android;

internal sealed class CameraLogic : BaseCameraLogic<GoogleMap>
{
    private readonly Action<LatLng> _updateVisibleRegion;

    public CameraLogic(Action<LatLng> updateVisibleRegion)
    {
        _updateVisibleRegion = updateVisibleRegion;
    }

    public override void Register(Map map, GoogleMap nativeMap)
    {
        base.Register(map, nativeMap);

        UnsubscribeCameraEvents(_nativeMap);

        nativeMap.CameraChange += OnNativeMapCameraChange;
        nativeMap.CameraMoveStarted += OnNativeMapCameraMoveStarted;
        nativeMap.CameraMove += OnNativeMapCameraMove;
        nativeMap.CameraIdle += OnNativeMapCameraIdle;
    }

    public override void Unregister()
    {
        UnsubscribeCameraEvents(_nativeMap);
        base.Unregister();
    }

    public override void OnMoveToRegionRequest(MoveToRegionMessage m)
    {
        if (_nativeMap == null)
        {
            return;
        }

        var span = m.Span;
        var animate = m.Animate;

        span = span.ClampLatitude(85, -85);
        var ne = new LatLng(span.Center.Latitude + span.LatitudeDegrees / 2, span.Center.Longitude + span.LongitudeDegrees / 2);
        var sw = new LatLng(span.Center.Latitude - span.LatitudeDegrees / 2, span.Center.Longitude - span.LongitudeDegrees / 2);
        var update = GCameraUpdateFactory.NewLatLngBounds(new LatLngBounds(sw, ne), 0);

        try
        {
            if (animate)
            {
                _nativeMap.AnimateCamera(update);
            }
            else
            {
                _nativeMap.MoveCamera(update);
            }
        }
        catch (IllegalStateException exc)
        {
            System.Diagnostics.Debug.WriteLine("MoveToRegion exception: " + exc);
        }
    }

    public override void OnMoveCameraRequest(CameraUpdateMessage m)
    {
        MoveCamera(m.Update);
        m.Callback.OnFinished();
    }

    public override void OnAnimateCameraRequest(CameraUpdateMessage m)
    {
        var update = m.Update.ToAndroid(ScaledDensity);
        var cancellableCallback = new DelegateCancelableCallback(
                m.Callback.OnFinished,
                m.Callback.OnCanceled);

        if (m.Duration.HasValue)
        {
            _nativeMap.AnimateCamera(update, (int)m.Duration.Value.TotalMilliseconds, cancellableCallback);
        }
        else
        {
            _nativeMap.AnimateCamera(update, cancellableCallback);
        }
    }

    internal void MoveCamera(CameraUpdate update)
    {
        _nativeMap.MoveCamera(update.ToAndroid(ScaledDensity));
    }

    private void UnsubscribeCameraEvents(GoogleMap nativeMap)
    {
        if (nativeMap == null)
        {
            return;
        }

        nativeMap.CameraChange -= OnNativeMapCameraChange;
        nativeMap.CameraMoveStarted -= OnNativeMapCameraMoveStarted;
        nativeMap.CameraMove -= OnNativeMapCameraMove;
        nativeMap.CameraIdle -= OnNativeMapCameraIdle;
    }

    private void OnNativeMapCameraChange(object sender, CameraChangeEventArgs e)
    {
        _updateVisibleRegion?.Invoke(e.Position.Target);
        var camera = e.Position.ToMaui();
        _map.CameraPosition = camera;
        _map.SendCameraChanged(camera);
    }

    private void OnNativeMapCameraMoveStarted(object sender, GoogleMap.CameraMoveStartedEventArgs e)
    {
        // see https://developers.google.com/maps/documentation/android-api/events#camera_change_events
        _map.SendCameraMoveStarted(e.Reason == OnCameraMoveStartedListener.ReasonGesture);
    }

    private void OnNativeMapCameraMove(object sender, System.EventArgs e)
    {
        _map.SendCameraMoving(_nativeMap.CameraPosition.ToMaui());
    }

    private void OnNativeMapCameraIdle(object sender, System.EventArgs e)
    {
        _map.SendCameraIdled(_nativeMap.CameraPosition.ToMaui());
    }
}