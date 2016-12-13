using System;
namespace Xamarin.Forms.GoogleMaps
{
    public class PinClickedEventArgs : EventArgs
    {
        public bool Handled { get; set; } = false;

        public Pin Pin { get; }

        public IPin Item { get; }


        internal PinClickedEventArgs(Pin pin, IPin item)
        {
            this.Pin = pin;
            Item = item;
        }
    }
}
