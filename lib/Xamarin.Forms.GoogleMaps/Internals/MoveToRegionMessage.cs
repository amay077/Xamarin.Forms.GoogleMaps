using System;

namespace Xamarin.Forms.GoogleMaps.Internals
{
    class MoveToRegionMessage
    {
        public MapSpan Span { get; private set; }
        public bool Animate { get; private set; }

        public MoveToRegionMessage(MapSpan mapSpan, bool animate = true)
        {
            this.Span = mapSpan;
            this.Animate = animate;
        }
    }
}


