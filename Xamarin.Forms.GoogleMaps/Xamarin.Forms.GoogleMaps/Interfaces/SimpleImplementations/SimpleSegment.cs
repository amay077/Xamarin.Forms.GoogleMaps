using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms.GoogleMaps.Interfaces.SimpleImplementations
{
    public class SimpleSegment : ISegment
    {
        public Position Position1 { get; set; }

        public Position Position2 { get; set; }

        //public static SimpleSegment operator /(SimpleSegment left, double fact) { return new SimpleSegment() { Position1 = left.Position1 / fact, Position2 = left.Position2 / fact }; }
    }
    public static class SimpleSegmentExtensions
    {


    }
}
