using System;

namespace Xamarin.Forms.GoogleMaps
{
    public class InfoWindowClickedEventArgs : EventArgs
    {
        public Pin Pin { get; }
        public IPin Item { get; }

        internal InfoWindowClickedEventArgs(Pin pin, IPin item)
        {
            this.Pin = pin;
            this.Item = item;
        }
    }
}
