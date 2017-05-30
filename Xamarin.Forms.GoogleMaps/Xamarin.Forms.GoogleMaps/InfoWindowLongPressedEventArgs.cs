using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps
{
    public sealed class InfoWindowLongPressedEventArgs : EventArgs
    {
        public Pin Pin { get; }

        internal InfoWindowLongPressedEventArgs(Pin pin)
        {
            this.Pin = pin;
        }
    }
}
