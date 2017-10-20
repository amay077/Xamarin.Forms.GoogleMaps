using System;

namespace Xamarin.Forms.GoogleMaps
{
    public sealed class CameraMovingEventArgs : EventArgs
    {
        public CameraPosition Position { get; }

        public CameraMovingEventArgs(CameraPosition position)
        {
            this.Position = position;
        }
    }
}