using System;
namespace Xamarin.Forms.GoogleMaps
{
    public class PinDragEventArgs : EventArgs
    {
        public Pin Pin
        {
            get;
            private set;
        }

        internal PinDragEventArgs(Pin pin)
        {
            this.Pin = pin;
        }
    }
}

