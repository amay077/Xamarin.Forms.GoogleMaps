
namespace Maui.GoogleMaps
{
    public sealed class PinClickedEventArgs : EventArgs
    {
        public bool Handled { get; set; } = false;
        public Pin Pin { get; }

        internal PinClickedEventArgs(Pin pin)
        {
            this.Pin = pin;
        }
    }
}
