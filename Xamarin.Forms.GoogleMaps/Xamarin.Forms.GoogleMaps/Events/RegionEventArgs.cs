using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps.Events
{
    public class RegionEventArgs:EventArgs
    {
        public MapSpan OldValue { get; set; }
        public MapSpan NewValue { get; set; }
    }
}
