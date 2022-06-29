
namespace Maui.GoogleMaps
{
    public sealed class SelectedPinChangedEventArgs : EventArgs
    {
        public Pin SelectedPin
        {
            get;
            private set;
        }

        internal SelectedPinChangedEventArgs(Pin selectedPin)
        {
            this.SelectedPin = selectedPin;
        }
    }
}

