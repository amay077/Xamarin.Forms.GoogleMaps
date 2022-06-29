
namespace Maui.GoogleMaps
{
    public sealed class MapLongClickedEventArgs : EventArgs
    {
        public Position Point { get; }

        internal MapLongClickedEventArgs(Position point)
        {
            this.Point = point;
        }
    }
}