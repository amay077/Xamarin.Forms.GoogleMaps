using System;
namespace Xamarin.Forms.GoogleMaps.Internals
{
    public interface IAnimationCallback
    {
        void OnFinished();
        void OnCanceled();
    }
}
