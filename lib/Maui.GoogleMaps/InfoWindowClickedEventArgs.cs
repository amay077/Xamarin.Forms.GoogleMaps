
namespace Maui.GoogleMaps
{
    public sealed class InfoWindowClickedEventArgs : EventArgs
    {
        public Pin Pin { get; }

        internal InfoWindowClickedEventArgs(Pin pin)
        {
            this.Pin = pin;
        }
    }
}