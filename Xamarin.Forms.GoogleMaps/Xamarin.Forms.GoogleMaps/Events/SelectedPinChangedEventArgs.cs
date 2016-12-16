using System;
namespace Xamarin.Forms.GoogleMaps
{
    public sealed class SelectedPinChangedEventArgs : EventArgs
    {
        public Pin SelectedPin
        {
            get;
            private set;
        }

        public IPin SelectedItem
        {
            get;
            private set;
        }

        internal SelectedPinChangedEventArgs(Pin selectedPin, IPin item)
        {
            this.SelectedPin = selectedPin;
            SelectedItem = item;
        }
    }
}

