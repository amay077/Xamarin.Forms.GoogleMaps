using System;

namespace Xamarin.Forms.GoogleMaps
{
    public class MapLongClickedEventArgs : EventArgs
    {
        public Position Point { get; }

        internal MapLongClickedEventArgs(Position point)
        {
            this.Point = point;
        }
    }
}
