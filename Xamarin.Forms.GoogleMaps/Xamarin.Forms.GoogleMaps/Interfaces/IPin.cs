using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Xamarin.Forms.GoogleMaps
{
    public interface IPin : INotifyPropertyChanged
    {
        string PinTitle { get; }

        string PinSubTitle { get; }

        /// <summary>
        /// PinPosition needs a setter to be bindable
        /// </summary>
        Position PinPosition { get; }

        float PinRotation { get; }

        BitmapDescriptor PinIcon { get; }

        bool PinIsDraggable { get; }

        IPinConfig PinConfig { get; }
    }
    public interface IPinConfig
    {
        PinType PinType { get; set; }

        ICommand PinClickedCommand { get; set; }

        object PinClickedCommandParameter { get; set; }

        ICommand InfoWindowClickedCommand { get; set; }

        object InfoWindowClickedCommandParameter { get; set; }

        ICommand PinSelectedCommand { get; set; }

        object PinSelectedCommandParameter { get; set; }

        ICommand PinDragStartCommand { get; set; }

        object PinDragStartCommandParameter { get; set; }

        ICommand PinDragEndCommand { get; set; }

        object PinDragEndCommandParameter { get; set; }

        ICommand PinDraggingCommand { get; set; }

        object PinDraggingCommandParameter { get; set; }

        AppearMarkerAnimation AppearAnimation { get; set; }

        Pin GMPin { get; set; }
        object NativePin { get; set; }
    }

    public static class IPinExtensions
    {
        public static Pin ToPin(this IPin iPin)
        {
            var pin = new Pin() { BindingContext = iPin };
            pin.SetBinding(Pin.LabelProperty, nameof(IPin.PinTitle));
            pin.SetBinding(Pin.AddressProperty, nameof(IPin.PinSubTitle));
            pin.SetBinding(Pin.IconProperty, nameof(IPin.PinIcon));
            pin.SetBinding(Pin.IsDraggableProperty, nameof(IPin.PinIsDraggable));
            pin.SetBinding(Pin.PositionProperty, nameof(IPin.PinPosition));
            pin.SetBinding(Pin.RotationProperty, nameof(IPin.PinRotation));
            if (iPin.PinConfig != null)
            {
                pin.SetBinding(Pin.TypeProperty, nameof(IPin.PinConfig) + "." + nameof(IPinConfig.PinType));
                pin.SetBinding(Pin.AppearAnimationProperty, nameof(IPin.PinConfig) + "." + nameof(IPinConfig.AppearAnimation));
            }
            return pin;
        }

    }
}
