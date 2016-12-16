using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps
{
    public interface ISegment
    {
        Position Position1 { get; }
        Position Position2 { get; }
    }
}
