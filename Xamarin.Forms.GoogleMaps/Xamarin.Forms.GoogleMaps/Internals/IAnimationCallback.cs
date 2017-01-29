using System;
namespace Xamarin.Forms.GoogleMaps.Internals
{
    internal interface IAnimationCallback
    {
        void OnFinished();
        void OnCanceled();
    }
}
