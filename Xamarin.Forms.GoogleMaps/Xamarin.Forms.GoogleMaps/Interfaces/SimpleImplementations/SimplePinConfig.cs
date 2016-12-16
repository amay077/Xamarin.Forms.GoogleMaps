using System;
using System.Windows.Input;

namespace Xamarin.Forms.GoogleMaps.Interfaces.SimpleImplementations
{
    public class SimplePinConfig : IPinConfig
    {
        public ICommand InfoWindowClickedCommand { get; set; }

        public object InfoWindowClickedCommandParameter { get; set; }

        public ICommand PinClickedCommand { get; set; }

        public object PinClickedCommandParameter { get; set; }

        public ICommand PinSelectedCommand { get; set; }

        public object PinSelectedCommandParameter { get; set; }

        public ICommand PinDragStartCommand { get; set; }

        public object PinDragStartCommandParameter { get; set; }

        public ICommand PinDragEndCommand { get; set; }

        public object PinDragEndCommandParameter { get; set; }

        public ICommand PinDraggingCommand { get; set; }

        public object PinDraggingCommandParameter { get; set; }

        public PinType PinType { get; set; } = PinType.Place;

        public AppearMarkerAnimation AppearAnimation { get; set; } = AppearMarkerAnimation.None;

        public Pin GMPin { get; set; }
        public object NativePin { get; set; }
    }
}
