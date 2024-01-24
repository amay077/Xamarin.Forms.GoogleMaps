
namespace Maui.GoogleMaps.Internals;

internal class DelegateAnimationCallback : IAnimationCallback
{
    readonly Action _onFinished;
    readonly Action _onCanceled;

    public DelegateAnimationCallback(Action onFinished, Action onCanceled)
    {
        _onFinished = onFinished;
        _onCanceled = onCanceled;
    }

    public void OnFinished()
    {
        _onFinished?.Invoke();
    }

    public void OnCanceled()
    {
        _onCanceled?.Invoke();
    }
}
