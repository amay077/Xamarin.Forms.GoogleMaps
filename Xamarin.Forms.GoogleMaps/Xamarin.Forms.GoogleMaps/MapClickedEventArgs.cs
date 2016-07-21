using System;

namespace Xamarin.Forms.GoogleMaps
{
    public class MapClickedEventArgs : EventArgs
    {
        public Position Point { get; }   

        internal MapClickedEventArgs(Position point)
        {
            this.Point = point;
        }
    }
}