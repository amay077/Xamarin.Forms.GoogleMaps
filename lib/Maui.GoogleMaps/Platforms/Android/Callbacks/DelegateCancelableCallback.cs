namespace Maui.GoogleMaps.Android.Callbacks;

internal class DelegateCancelableCallback : Java.Lang.Object, global::Android.Gms.Maps.GoogleMap.ICancelableCallback
{
    private readonly Action _onFinish;
    private readonly Action _onCancel;

    public DelegateCancelableCallback(Action onFinish, Action onCancel)
    {
        _onFinish = onFinish;
        _onCancel = onCancel;
    }

    public void OnFinish()
    {
        _onFinish?.Invoke();
    }

    public void OnCancel()
    {
        _onCancel?.Invoke();
    }
}