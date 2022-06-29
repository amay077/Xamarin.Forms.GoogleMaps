
namespace Maui.GoogleMaps
{
    public sealed class InfoWindowLongClickedEventArgs : EventArgs
    {
        public Pin Pin { get; }

        internal InfoWindowLongClickedEventArgs(Pin pin)
        {
            this.Pin = pin;
        }
    }
}
