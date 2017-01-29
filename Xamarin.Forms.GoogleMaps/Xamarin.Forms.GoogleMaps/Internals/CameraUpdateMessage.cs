using System;
namespace Xamarin.Forms.GoogleMaps.Internals
{
    internal sealed class CameraUpdateMessage
    {
        public CameraUpdate Update { get; }

        public IAnimationCallback Callback { get; }

        public CameraUpdateMessage(CameraUpdate update, IAnimationCallback callback)
        {
            Update = update;
            Callback = callback;
        }
    }
}
