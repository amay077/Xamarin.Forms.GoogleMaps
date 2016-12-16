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

        public IPin Item
        {
            get;
            private set;
        }

        internal PinDragEventArgs(Pin pin, IPin item)
        {
            this.Pin = pin;
            Item = item;
        }
    }
}

