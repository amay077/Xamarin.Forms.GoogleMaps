using System;

namespace Xamarin.Forms.GoogleMaps
{
    public class SelectedItemChangedEventArgs : EventArgs
    {
        public SelectedItemChangedEventArgs(object item)
        {
            Item = item;
        }

        public object Item { get; }
    }
}
